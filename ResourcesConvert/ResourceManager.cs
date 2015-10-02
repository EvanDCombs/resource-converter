using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ResourcesConvert
{
    public class ResourceManager
    {
        private Type resourceType;
        private List<Property> Properties;
        public ObservableCollection<dynamic> Resources;

        public ResourceManager()
        {
            Properties = new List<Property>();
            Properties.Add(new Property("Name", typeof(string)));
            Properties.Add(new Property("Default", typeof(string)));
            Resources = new ObservableCollection<dynamic>();
            resourceType = ResourceTypeGenerator.CreateResourceType(Properties);
        }
        public void AddResourceItem()
        {
            Resources.Add(ResourceTypeGenerator.CreateResourceObject(resourceType));
        }
        public ObservableCollection<dynamic> AddColumn(string header)
        {
            //records list of properties from previous iteration of Resource
            PropertyInfo[] oldProperties = resourceType.GetProperties();
            //adds new property to Properties list
            Properties.Add(new Property(header, typeof(string)));
            //creates the new iteration of Resource
            resourceType = ResourceTypeGenerator.CreateResourceType(Properties);
            //gets list of current properties for Resource
            PropertyInfo[] newProperties = resourceType.GetProperties();

            //temporary collection to store new Resources
            ObservableCollection<dynamic> temp = new ObservableCollection<dynamic>();
            //iterates list of Resources before conversion
            foreach (dynamic oldResource in Resources)
            {
                //creates a new object from the new iteration of Resource
                dynamic newResource = ResourceTypeGenerator.CreateResourceObject(resourceType);
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
            Resources = temp;
            return Resources;
        }
    }
}
