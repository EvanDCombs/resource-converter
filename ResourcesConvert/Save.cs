using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Reflection;
using System.Text;
using System.IO;
using ResourcesConvert.Properties;

namespace ResourcesConvert
{
    public static class Save
    {
        /*
        XmlScript = Android, Windows
        StringBuilder = iOS
        */

        private delegate XmlScript CreateXmlScript();
        private delegate void CreateXmlScriptResource(string key, KeyValuePair<string, string> pair, XmlScript resourceFile);
        private delegate void CreateStringBuilderResource(string key, KeyValuePair<string, string> pair, StringBuilder resourceFile);
        private delegate string CreateGeneratedCSharp(string key);

        public static void AndroidResource(string filepath, ObservableCollection<dynamic> resources)
        {
            filepath = filepath + "//Android";
            //gets all properties of a Resource
            PropertyInfo[] properties = Resource.Properties;
            //creates a dictionary of XmlScripts for each language
            Dictionary<string, XmlScript> resourceFiles = CreateResourceFilesDictionary(properties, CreateAndroidXmlScript);
            //converts resources into dictionaries for easier manipulation
            List<Dictionary<string, string>> resourceList = ResourceListToDictionaryList(resources);
            //adds each resource to the appropriate ResourceFile
            CreateResources(resourceList, resourceFiles, CreateAndroidResource);

            SaveFiles(filepath, "//strings-", ".xml", resourceFiles);
        }
        public static void iOSResources(string filepath, string cSharpNameSpace, ObservableCollection<dynamic> resources)
        {
            filepath = filepath + "//iOS";
            //gets all properties of a Resource
            PropertyInfo[] properties = Resource.Properties;
            //creates a dictionary of XmlScripts to use for each property
            Dictionary<string, StringBuilder> resourceFiles = CreateResourceFilesDictionary(properties);
            //converts resources into dictionaries for easier manipulation
            List<Dictionary<string, string>> resourceList = ResourceListToDictionaryList(resources);
            //adds each resource to the appropriate ResourceFile
            CreateResources(resourceList, resourceFiles, CreateiOSResource);

            SaveFiles(filepath, "//Localizable_", ".strings", resourceFiles);

            StringBuilder generated = GenerateCSharp(resourceList, "GeneratediOSBoilerplate.txt", cSharpNameSpace, GenerateiOSCSharp);
            SaveGeneratedCSharp(filepath, "//Strings", generated);
        }
        public static void WinResource(string filepath, string cSharpNameSpace, ObservableCollection<dynamic> resources)
        {
            filepath = filepath + "//Win";
            //gets all properties of a Resource
            PropertyInfo[] properties = Resource.Properties;
            //creates a dictionary of XmlScripts to use for each property
            Dictionary<string, XmlScript> resourceFiles = CreateResourceFilesDictionary(properties, CreateWinXmlScript);
            //converts resources into dictionaries for easier manipulation
            List<Dictionary<string, string>> resourceList = ResourceListToDictionaryList(resources);
            CreateResources(resourceList, resourceFiles, CreateWinResource);

            SaveFiles(filepath, "//strings-", ".resx", resourceFiles);

            StringBuilder generated = GenerateCSharp(resourceList, "GeneratedWinBoilerplate.txt", cSharpNameSpace, GenerateWinCSharp);
            SaveGeneratedCSharp(filepath, "//Strings", generated);
        }
        public static void ConvertableResource(string filepath, ObservableCollection<dynamic> resources)
        {
            XmlScript xmlScript = new XmlScript();
            XmlElement rootElement = XmlScript.XmlRoot(xmlScript, "resource-list");
            foreach (dynamic resource in resources)
            {
                XmlElement resourceElement = XmlScript.XmlElement(xmlScript, rootElement, "resource");
                PropertyInfo[] properties = Resource.Properties;
                foreach (PropertyInfo property in properties)
                {
                    string data = property.GetValue(resource);
                    string name = property.Name;
                    XmlScript.XmlElementWithText(xmlScript, resourceElement, name, data);
                }
                rootElement.AppendChild(resourceElement);
            }
            xmlScript.Save(filepath);
        }

        private static Dictionary<string, StringBuilder> CreateResourceFilesDictionary(PropertyInfo[] properties)
        {
            Dictionary<string, StringBuilder> dictionary = new Dictionary<string, StringBuilder>();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "Name")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    dictionary.Add(property.Name, stringBuilder);
                }
            }
            return dictionary;
        }
        private static Dictionary<string, XmlScript> CreateResourceFilesDictionary(PropertyInfo[] properties, CreateXmlScript createXmlScript)
        {
            Dictionary<string, XmlScript> dictionary = new Dictionary<string, XmlScript>();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "Name")
                {
                    dictionary.Add(property.Name, createXmlScript());
                }
            }

            return dictionary;
        }

        private static XmlScript CreateAndroidXmlScript()
        {
            XmlScript xmlScript = new XmlScript();
            XmlElement rootElement = XmlScript.XmlRoot(xmlScript, "resources");
            return xmlScript;
        }
        private static XmlScript CreateWinXmlScript()
        {
            XmlScript xmlScript = new XmlScript();
            Uri uri = new Uri("/WinResource.xml", UriKind.RelativeOrAbsolute);
            using (Stream stream = System.Windows.Application.GetResourceStream(uri).Stream)
            {
                xmlScript.Load(stream);
            }
            return xmlScript;
        }

        private static List<Dictionary<string, string>> ResourceListToDictionaryList(ObservableCollection<dynamic> resources)
        {
            //converts resources into dictionaries for easier manipulation
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (dynamic resource in resources)
            {
                list.Add(Resource.ToDictionary(resource));
            }
            return list;
        }

        private static void CreateResources(List<Dictionary<string, string>> resourceList, Dictionary<string, XmlScript> resourceFiles, CreateXmlScriptResource createXmlScriptResource)
        {
            foreach (Dictionary<string, string> dictionary in resourceList)
            {
                string key = dictionary["Name"];
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    if (pair.Key != "Name")
                    {
                        createXmlScriptResource(key, pair, resourceFiles[pair.Key]);
                    }
                }
            }
        }
        private static void CreateResources(List<Dictionary<string, string>> resourceList, Dictionary<string, StringBuilder> resourceFiles, CreateStringBuilderResource createStringBuilderResource)
        {
            foreach (Dictionary<string, string> dictionary in resourceList)
            {
                string key = dictionary["Name"];
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    if (pair.Key != "Name")
                    {
                        createStringBuilderResource(key, pair, resourceFiles[pair.Key]);
                    }
                }
            }
        }

        private static void CreateAndroidResource(string key, KeyValuePair<string, string> pair, XmlScript resourceFile)
        {
            XmlScript script = resourceFile;
            XmlScript.XmlElementWithAttributeAndText(script, script.DocumentElement, "string", pair.Value, "name", key);
        }
        private static void CreateiOSResource(string key, KeyValuePair<string, string> pair, StringBuilder resourceFile)
        {
            StringBuilder stringBuilder = resourceFile;
            stringBuilder.AppendLine("\"" + key + "\"=\"" + pair.Value + "\";");
        }
        private static void CreateWinResource(string key, KeyValuePair<string, string> pair, XmlScript resourceFile)
        {
            XmlScript script = resourceFile;
            XmlElement dataElement = XmlScript.XmlElement(script, script.DocumentElement, "data");
            XmlScript.XmlAttribute(dataElement, "name", key);
            XmlScript.XmlAttribute(dataElement, "xml:space", "preserve");
            XmlScript.XmlElementWithText(script, dataElement, "value", pair.Value);
        }

        private static StringBuilder GenerateCSharp(List<Dictionary<string, string>> resourceList, string filename, string csNamespace, CreateGeneratedCSharp generateCSharp)
        {
            StringBuilder csharp = new StringBuilder();
            csharp.AppendLine("");
            foreach (Dictionary<string, string> dictionary in resourceList)
            {
                string key = dictionary["Name"];
                string generated = generateCSharp(key);
                csharp.AppendLine(generated);
            }
            StringBuilder boilerplate = new StringBuilder();
            Uri uri = new Uri(filename, UriKind.RelativeOrAbsolute);
            using (Stream stream = System.Windows.Application.GetResourceStream(uri).Stream)
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    boilerplate.Append(streamReader.ReadToEnd());
                }
            }
            boilerplate.Replace("[NAMESPACE]", csNamespace);
            boilerplate.Replace("[PLACE CSHARP HERE]", csharp.ToString());
            return boilerplate;
        }

        private static string GenerateiOSCSharp(string key)
        {
           return Indent(2) + "public static string " + key + " { get { return NSBundle.MainBundle.LocalizedString(" + key + ", null); } }";
        }
        private static string GenerateWinCSharp(string key)
        {
            return Indent(2) + "internal static string " + key + " { get { return ResourceManager.GetString(" + key + ", resourceCulture); } }";
        }

        private static void SaveFiles(string filepath, string filename, string extension, Dictionary<string, XmlScript> resourceFiles)
        {
            FileManager.CreateDirectory(filepath);
            foreach (KeyValuePair<string, XmlScript> pair in resourceFiles)
            {
                pair.Value.Save(filepath + filename + pair.Key + extension);
            }
        }
        private static void SaveFiles(string filepath, string filename, string extension, Dictionary<string, StringBuilder> resourceFiles)
        {
            FileManager.CreateDirectory(filepath);
            foreach (KeyValuePair<string, StringBuilder> pair in resourceFiles)
            {
                FileManager.WriteLocalFile(filepath + filename + pair.Key + extension, pair.Value.ToString());
            }
        }
        private static void SaveGeneratedCSharp(string filepath, string filename, StringBuilder generatedCSharp)
        {
            FileManager.CreateDirectory(filepath);
            FileManager.WriteLocalFile(filepath + filename + ".cs", generatedCSharp.ToString());
        }



        private static string Indent(int count)
        {
            return new string(' ', count * 4);
        }
    }
}
