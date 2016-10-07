using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ResourcesConvert
{
    public class ConvertToConvertable : ConvertTo
    {
        #region Properties
        protected override string ResourceFileName { get { return "//"; } }
        protected override string CSharpFileName { get { return "Strings"; } }
        protected override string Folder { get { return "//Shared"; } }
        protected override string GetString { get { return "GetStringPartial(name, ref value"; } }
        protected override string ResourceFileExtention { get { return ""; } }
        protected override StringBuilder UsingStatements { get { return new StringBuilder("using System;"); } }
        #endregion
        #region Initialization
        private static ConvertToConvertable instance;
        public static ConvertToConvertable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConvertToConvertable(Resource.Properties);
                }
                return instance;
            }
        }
        private ConvertToConvertable(PropertyInfo[] properties) : base(properties){ }
        #endregion
        #region Methods
        public override void CreateFiles(string filepath, string nameSpace, System.Collections.ObjectModel.ObservableCollection<dynamic> resources)
        {
            XmlScript xmlScript = new XmlScript();
            XmlElement rootElement = XmlScript.XmlRoot(xmlScript, "resource-list");
            XmlScript.XmlAttribute(rootElement, "namespace", nameSpace);
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
        protected override string GetResourceString(string key, string value)
        {
            return "";
        }
        #endregion
    }
}
