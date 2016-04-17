using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesConvert
{
    public abstract class ConvertTo
    {
        #region Const Fields
        protected const string XML_EXTENSION = ".xml";
        protected const string STRINGS_EXTENSION = ".strings";
        protected const string RESX_EXTENSION = ".resx";
        protected const string CSHARP_EXTENSION = ".cs";
        protected const string PROPERTY_NAME = "Name";
        protected const string NAMESPACE = "[NAMESPACE]";
        protected const string CSHARP = "[CSHARP]";
        protected const string GET_STRING = "[GET STRING]";
        protected const string USING_STATEMENTS = "[USING STATEMENTS]";
        protected const string DEPENDENCIES = "[DEPENDENCIES]";
        #endregion
        #region Properties
        protected abstract string ResourceFileName { get; }
        protected abstract string CSharpFileName { get; }
        protected abstract string Folder { get; }
        protected abstract string GetString { get; }
        protected abstract StringBuilder UsingStatements { get; }
        protected virtual string ResourceFileExtention { get { return XML_EXTENSION; } }
        protected virtual string Dependencies { get { return ""; } }
        public StringBuilder CSharpText { get; protected set; }
        protected Dictionary<string, StringBuilder> Languages { get; set; }
        #endregion
        #region Initialization
        public ConvertTo(PropertyInfo[] properties)
        {
            //Gets every language from the properties array passed in and adds it to a Dictionary
            Languages = new Dictionary<string, StringBuilder>();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name != PROPERTY_NAME)
                {
                    Languages.Add(property.Name, null);
                }
            }
        }
        #endregion
        #region Methods
        private List<Dictionary<string, string>> ResourceListToDictionaryList(ObservableCollection<dynamic> resources)
        {
            //converts resources into dictionaries for easier manipulation
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (dynamic resource in resources)
            {
                list.Add(Resource.ToDictionary(resource));
            }
            return list;
        }
        protected string Indent(float count)
        {
            return new string(' ', (int)(count * 4));
        }
        #endregion
        #region Virtual Methods
        public virtual void CreateFiles(string filepath, string nameSpace, ObservableCollection<dynamic> resources)
        {
            //converts resources into dictionaries for easier manipulation
            List<Dictionary<string, string>> list = ResourceListToDictionaryList(resources);
            CreateFiles(filepath, nameSpace, list);
        }
        public virtual void CreateFiles(string filepath, string nameSpace, List<Dictionary<string, string>> resources)
        {
            //Retrieves Language Keys then iterates over each Language create a StringBuilder containing formated resources
            Dictionary<string, StringBuilder>.KeyCollection languageKeys = Languages.Keys;
            for (int i = 0; i < languageKeys.Count; i++)
            {
                Languages[languageKeys.ElementAt(i)] = CreateResourceFile(resources);
            }

            StringBuilder cSharpFile = CreateCSharpFile(resources, nameSpace);

            SaveFiles(filepath, Languages, cSharpFile);
        }
        protected virtual StringBuilder CreateResourceFile(List<Dictionary<string, string>> resources)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");

            foreach (Dictionary<string, string> dictionary in resources)
            {
                string key = dictionary[PROPERTY_NAME];
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    if (pair.Key != PROPERTY_NAME)
                    {
                        sb.AppendLine(GetResourceString(key, pair.Value));
                    }
                }
            }

            return sb;
        }
        protected virtual StringBuilder CreateCSharpFile(List<Dictionary<string, string>> resources, string nameSpace)
        {
            StringBuilder sb = new StringBuilder();

            Uri uri = new Uri("csharp_strings.txt", UriKind.RelativeOrAbsolute);
            using (Stream stream = System.Windows.Application.GetResourceStream(uri).Stream)
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    sb.Append(streamReader.ReadToEnd());
                }
            }

            sb.Replace(DEPENDENCIES, Dependencies);
            sb.Replace(NAMESPACE, nameSpace);
            sb.Replace(GET_STRING, GetString);
            sb.Replace(USING_STATEMENTS, UsingStatements.ToString());

            return sb;
        }
        protected virtual void SaveFiles(string filepath, Dictionary<string, StringBuilder> resourceFiles, StringBuilder cSharpFile)
        {
            foreach (KeyValuePair<string, StringBuilder> pair in resourceFiles)
            {
                SaveFile(filepath + Folder, ResourceFileName + pair.Key, ResourceFileExtention, pair.Value);
            }
            SaveFile(filepath + Folder, CSharpFileName, CSHARP_EXTENSION, cSharpFile);
        }
        protected virtual void SaveFile(string filepath, string filename, string extension, StringBuilder data)
        {
            FileManager.CreateDirectory(filepath);
            FileManager.WriteLocalFile(filepath + filename + extension, data.ToString());
        }
        #endregion
        #region abstract Methods
        protected abstract string GetResourceString(string key, string value);
        #endregion
    }
}
