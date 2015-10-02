using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace ResourcesConvert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ResourceManager resourceManager;

        public MainWindow()
        {
            InitializeComponent();
            resourceManager = new ResourceManager();
            dataGrid.ItemsSource = resourceManager.Resources;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch ((string)button.Tag)
            {
                case "new":
                    break;
                case "open":
                    break;
                case "save":
                    break;
                case "convert":
                    break;
                case "add_column":
                    dataGrid.ItemsSource = resourceManager.AddColumn(textBox.Text);
                    textBox.Clear();
                    break;
                case "add_row":
                    resourceManager.AddResourceItem();
                    break;
            }
        }
        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null) child = GetVisualChild<T>(v);
                if (child != null) break;
            }
            return child;
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataGridRow dataGridRow = e.Row;
            if (dataGridRow != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(dataGridRow);
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(0);
                if (cell != null)
                {
                    DataGridColumn column = e.Column;
                    string text = ((TextBox)cell.Content).Text;
                    int row = dataGridRow.GetIndex();
                    string language = (string)column.Header;
                    //resourceManager.ChangeResource(row, language, text);
                }
            }
        }

        private void dataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(dataGrid);
            DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(0);
            cell.IsEditing = false;
        }
    }
}
