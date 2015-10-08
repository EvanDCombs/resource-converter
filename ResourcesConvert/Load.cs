using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace ResourcesConvert
{
    public static class Load
    {
        public static ObservableCollection<dynamic> ConvertableResource(string filepath)
        {
            XmlDocument xmlDocument = FileManager.RetrieveLocalXml(filepath);
            XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
            xPathNavigator.MoveToRoot();
            xPathNavigator.MoveToFirstChild(); //resource-list
            if (xPathNavigator.MoveToFirstChild()) //resource
            {
                ObservableCollection<dynamic> temp = new ObservableCollection<dynamic>();

                // Get Data from first child, convert it into a dynamic bindable type, convert dict to new dynamic bindable type, and add to temp list
                Dictionary<string, string> dict = ResourceDataToDictionary(xPathNavigator);
                Resource.CreateType(dict, false);
                temp.Add(Resource.FromDictionary(dict));

                while (xPathNavigator.MoveToNext())
                {
                    // get data from child, convert dict to new dynamic bindable type, and add to temp list
                    dict = ResourceDataToDictionary(xPathNavigator);
                    temp.Add(Resource.FromDictionary(dict));
                }

                return temp;
            }
            return null;
        }
        //creates a dictionary of values found in XML
        private static Dictionary<string, string> ResourceDataToDictionary(XPathNavigator xPathNavigator)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (xPathNavigator.MoveToFirstChild()) //children
            {
                dict.Add(xPathNavigator.Name, xPathNavigator.Value);
                while (xPathNavigator.MoveToNext())
                {
                    dict.Add(xPathNavigator.Name, xPathNavigator.Value);
                }
            }

            return dict;
        }
    }
}
