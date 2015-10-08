using System;
using System.Collections.Generic;
using System.Reflection;

namespace ResourcesConvert
{
    public static class Resource
    {
        #region Static Fields
        private static Type type;
        private static List<Property> propertyList;
        #endregion
        #region Old Type Fields
        private static PropertyInfo[] oldProperties;
        #endregion
        #region Static Properties
        public static dynamic NewResource { get { return Activator.CreateInstance(Type); } }
        public static Type Type { get { return type; } }
        public static PropertyInfo[] Properties { get { return Type.GetProperties(); } }
        public static List<string> PropertyNames
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Property property in propertyList)
                {
                    names.Add(property.Name);
                }
                return names;
            }
        }
        #endregion
        #region Type Methods
        public static void RemoveProperty(int index)
        {
            propertyList.RemoveAt(index);
            CreateType();
        }
        public static void AddProperty(Property property)
        {
            propertyList.Add(property);
            CreateType();
        }
        public static void CreateType(Dictionary<string, string> dictionary, bool append)
        {
            Property[] properties = new Property[dictionary.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                properties[i] = new Property(pair.Key, typeof(string));
                i++;
            }
            CreateType(properties, append);
        }
        public static void CreateType(List<Property> properties, bool append)
        {
            CreateType(properties.ToArray(), append);
        }
        public static void CreateType(bool append, params Property[] properties)
        {
            CreateType(properties, append);
        }
        public static void CreateType(Property[] properties, bool append)
        {
            if (!append)
            {
                propertyList = new List<Property>();
            }
            foreach (Property property in properties)
            {
                propertyList.Add(property);
            }
            CreateType();
        }
        public static void CreateType()
        {
            if (type != null)
            {
                oldProperties = type.GetProperties();
            }
            type = TypeGenerator.CreateResourceType(propertyList, "Resource");
        }
        #endregion
        #region To Methods
        public static void ToDefaultType()
        {
            Property nameProperty = new Property("Name", typeof(string));
            Property defaultProperty = new Property("Default", typeof(string));
            CreateType(false, nameProperty, defaultProperty);
        }
        public static dynamic ToFreshType(dynamic oldResource)
        {
            dynamic newResource = NewResource;
            foreach (PropertyInfo oldProperty in oldProperties)
            {
                foreach (PropertyInfo newProperty in Properties)
                {
                    if (oldProperty.Name == newProperty.Name)
                    {
                        newProperty.SetValue(newResource, oldProperty.GetValue(oldResource));
                        break;
                    }
                }
            }
            return newResource;
        }
        public static Dictionary<string, string> ToDictionary(dynamic resource)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (PropertyInfo property in Properties)
            {
                string name = property.Name;
                string value = property.GetValue(resource);
                dictionary.Add(name, value);
            }

            return dictionary;
        }
        #endregion
        #region From Methods
        public static dynamic FromDictionary(Dictionary<string, string> dictionary)
        {
            dynamic resource = NewResource;
            foreach (PropertyInfo property in Properties)
            {
                property.SetValue(resource, dictionary[property.Name]);
            }
            return resource;
        }
        #endregion
    }
}
