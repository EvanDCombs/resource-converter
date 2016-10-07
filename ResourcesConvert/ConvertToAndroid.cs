using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ResourcesConvert
{
    public class ConvertToAndroid : ConvertTo
    {
        #region Const Fields
        private const string RESOURCES = "resources";
        #endregion
        #region Properties
        protected override string ResourceFileName { get { return "//strings-"; } }
        protected override string CSharpFileName { get { return "//Strings"; } }
        protected override string Folder { get { return "//Android"; } }
        protected override string GetString { get { return "value = Context.Resources.GetString(Context.Resources.GetIdentifier(name, \"string\", Context.PackageName));"; } }
        protected override string Dependencies { get { return "public static Context Context { get; set;}"; } }
        protected override StringBuilder UsingStatements { get { return new StringBuilder("using Android.Content;"); } }
        #endregion
        #region Initialization
        private static ConvertToAndroid instance;
        public static ConvertToAndroid Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConvertToAndroid(Resource.Properties);
                }
                return instance;
            }
        }
        private ConvertToAndroid(PropertyInfo[] properties) : base(properties)
        {
        }
        #endregion
        #region Methods
        protected override string GetResourceString(string key, string value)
        {
            return Indent(1) + "<string name=\"" + key + "\">" + value + "</string>";
        }
        protected override void SaveFile(string filepath, string filename, string extension, StringBuilder data)
        {
            if (extension != CSHARP_EXTENSION)
            {
                FileManager.CreateDirectory(filepath);
                XmlScript xml = ConvertToXml(data);
                xml.Save(filepath + filename + extension);
            }
            else
            {
                base.SaveFile(filepath, filename, extension, data);
            }
        }
        private XmlScript ConvertToXml(StringBuilder stringBuilder)
        {
            //Create Root XML
            XmlScript xmlScript = new XmlScript();
            XmlElement rootElement = XmlScript.XmlRoot(xmlScript, RESOURCES);

            //Add StringBuilder Lines to XML
            XmlDocumentFragment fragment = xmlScript.CreateDocumentFragment();
            fragment.InnerXml = stringBuilder.ToString();
            rootElement.AppendChild(fragment);

            return xmlScript;
        }
        #endregion
    }
}
