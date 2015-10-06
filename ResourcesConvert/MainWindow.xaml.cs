using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace ResourcesConvert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private Type bindableType;
        private List<Property> properties;
        private ObservableCollection<dynamic> resources;
        #endregion
        #region Properties
        private dynamic NewResource { get { return ResourceTypeGenerator.CreateResourceObject(bindableType); } }
        #endregion
        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
            resources = new ObservableCollection<dynamic>();
            properties = new List<Property>();
            properties.Add(new Property("Name", typeof(string)));
            properties.Add(new Property("Default", typeof(string)));
            bindableType = ResourceTypeGenerator.CreateResourceType(properties);
            resources.Add(NewResource);
            dataGrid.ItemsSource = resources;
        }
        #endregion
        #region Methods
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
                    AddColumn(textBox.Text);
                    dataGrid.ItemsSource = resources;
                    textBox.Clear();
                    break;
                case "add_row":
                    resources.Add(NewResource);
                    break;
            }
        }
        public void AddColumn(string column)
        {
            PropertyInfo[] oldProperties = bindableType.GetProperties();

            properties.Add(new Property(column, typeof(string)));
            bindableType = ResourceTypeGenerator.CreateResourceType(properties);
            PropertyInfo[] newProperties = bindableType.GetProperties();

            ObservableCollection<dynamic> temp = new ObservableCollection<dynamic>();
            foreach (dynamic oldResource in resources)
            {
                dynamic newResource = NewResource;
                foreach (PropertyInfo oldProperty in oldProperties)
                {
                    foreach (PropertyInfo newProperty in newProperties)
                    {
                        if (oldProperty.Name == newProperty.Name)
                        {
                            newProperty.SetValue(newResource, oldProperty.GetValue(oldResource));
                            break;
                        }
                    }
                }
                temp.Add(newResource);
            }

            resources = temp;
        }
        private void DockPanel_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dataGrid.CommitEdit();
        }
        private void SaveAsAndroidResource()
        { }
        private void SaveAsIOSResources()
        { }
        private void SaveAsWinResource()
        { }
        private void SaveAsConvertableResource()
        {

        }
        #endregion
    }
}
