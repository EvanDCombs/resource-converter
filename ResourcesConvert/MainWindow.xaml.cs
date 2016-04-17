using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ResourcesConvert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Const Fields
        private const string EXTENSION = ".xrc";
        #endregion
        #region Fields
        private bool IsEditing;
        private bool IsDragging;
        private string currentFile;
        private dynamic DraggedItem;
        private DataGridColumn column;
        private ObservableCollection<dynamic> resources;
        #endregion
        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
            resources = new ObservableCollection<dynamic>();
            EmptyDataGrid();
        }
        #endregion
        #region UI Methods
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch ((string)button.Tag)
            {
                case "new":
                    EmptyDataGrid();
                    break;
                case "open":
                    currentFile = FileManager.OpenFileLocator(false, EXTENSION)[0];
                    resources = Load.ConvertableResource(currentFile);
                    dataGrid.ItemsSource = resources;
                    SetComboBoxItems(Resource.PropertyNames);
                    break;
                case "save":
                    if (string.IsNullOrEmpty(currentFile))
                    {
                        currentFile = FileManager.SaveFileLocator(EXTENSION);
                    }
                    Save.ConvertableResource(currentFile, resources);
                    break;
                case "convert":
                    if (string.IsNullOrEmpty(currentFile))
                    {
                        currentFile = FileManager.SaveFileLocator(EXTENSION);
                    }
                    ConvertToConvertable.Instance.CreateFiles(currentFile, "", resources);
                    string folderPath = FileManager.OpenDirectoryLocator();
                    ConvertToAndroid.Instance.CreateFiles(currentFile, namespaceTextBox.Text, resources);
                    ConvertToiOS.Instance.CreateFiles(currentFile, namespaceTextBox.Text, resources);
                    ConvertToWin.Instance.CreateFiles(currentFile, namespaceTextBox.Text, resources);
                    ConvertToShared.Instance.CreateFiles(currentFile, namespaceTextBox.Text, resources);
                    break;
                case "add_column":
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        AddColumn(textBox.Text);
                        textBox.Clear();
                    }
                    break;
                case "add_row":
                    resources.Add(Resource.NewResource);
                    break;
                case "delete_row":
                    resources.Remove(dataGrid.SelectedCells[0].Item);
                    break;
                case "sort_row":
                    List<dynamic> list = resources.OrderBy(x => x.Name).ToList<dynamic>();
                    ObservableCollection<dynamic> oc = new ObservableCollection<dynamic>();
                    foreach (dynamic item in list)
                    {
                        oc.Add(item);
                    }
                    resources = oc;
                    dataGrid.ItemsSource = resources;
                    break;
                case "delete_column":
                    DeleteColumn(comboBox.SelectedIndex);
                    break;
            }
        }
        private void AddColumn(string column)
        {
            Resource.AddProperty(new Property(column, typeof(string)));
            ConvertToFreshResourceType();
        }
        private void DeleteColumn(int index)
        {
            int propertyIndex = index + 2;
            Resource.RemoveProperty(propertyIndex);
            ConvertToFreshResourceType();
        }
        private void ConvertToFreshResourceType()
        {
            ObservableCollection<dynamic> temp = new ObservableCollection<dynamic>();
            foreach (dynamic oldResource in resources)
            {
                temp.Add(Resource.ToFreshType(oldResource));
            }
            resources = temp;
            SetComboBoxItems(Resource.PropertyNames);
            dataGrid.ItemsSource = resources;
        }
        private void DockPanel_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dataGrid.CommitEdit();
        }
        private void EmptyDataGrid()
        {
            Resource.ToDefaultType();
            resources = new ObservableCollection<dynamic>();
            resources.Add(Resource.NewResource);
            dataGrid.ItemsSource = resources;
            SetComboBoxItems(Resource.PropertyNames);
        }
        private void SetComboBoxItems(List<string> columns)
        {
            comboBox.Items.Clear();
            for (int i = 2; i < columns.Count; i++)
            {
                comboBox.Items.Add(columns[i]);
            }
        }
        #endregion
        #region Drag and Drop
        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsEditing = true;
            if (IsDragging)
            {
                ResetDragDrop();  
            }
        }
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsEditing = false;
        }
        private void ResetDragDrop()
        {
            IsDragging = false;
            dataGrid.IsReadOnly = false;
        }
        private void dataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsEditing)
            {
                return;
            }
            DataGridRow row = UIHelper.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(dataGrid));
            if (row != null)
            {
                IsDragging = true;
                DraggedItem = (dynamic)row.Item;
            }
        }
        private void dataGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsDragging)
            {
                dataGrid.IsReadOnly = true;
                DataGridRow row = UIHelper.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(dataGrid));
                dynamic targetItem = (dynamic)row.Item;
                if (targetItem != null && !ReferenceEquals(DraggedItem, targetItem))
                {
                    int index = resources.IndexOf(targetItem);
                    resources.Remove(DraggedItem);
                    resources.Insert(index, DraggedItem);
                }
            }
            ResetDragDrop();
        }
        #endregion
    }
}
