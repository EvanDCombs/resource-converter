using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string currentFile;
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
                    Load.ConvertableResource(currentFile);
                    dataGrid.ItemsSource = resources;
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
                    Save.ConvertableResource(currentFile, resources);
                    string folderPath = FileManager.OpenDirectoryLocator();
                    Save.AndroidResource(folderPath, resources);
                    Save.iOSResources(folderPath, resources);
                    Save.WinResource(folderPath, resources);
                    break;
                case "add_column":
                    AddColumn(textBox.Text);
                    textBox.Clear();
                    break;
                case "add_row":
                    resources.Add(Resource.NewResource);
                    break;
                case "delete_row":
                    resources.Remove(dataGrid.SelectedCells[0].Item);
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
    }
}
