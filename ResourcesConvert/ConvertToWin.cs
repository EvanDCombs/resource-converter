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
    public class ConvertToWin : ConvertTo
    {
        #region Properties
        protected override string ResourceFileName { get { return "//strings-"; } }
        protected override string CSharpFileName { get { return "Strings"; } }
        protected override string Folder { get { return "//Win"; } }
        protected override string GetString { get { return "ResourceManager.GetString(\"name\", resourceCulture);"; } }
        protected override StringBuilder UsingStatements { get { return new StringBuilder("using System"); } }
        #endregion
        #region Initialization
        private static ConvertToWin instance;
        public static ConvertToWin Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConvertToWin(Resource.Properties);
                }
                return instance;
            }
        }
        private ConvertToWin(PropertyInfo[] properties) : base(properties){ }
        #endregion
        #region Methods
        protected override string GetResourceString(string key, string value)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<data name=\"" + key + "\" xml:space=\"preserve\">");
            sb.AppendLine("<value>" + value + "</value>");
            sb.AppendLine("</data>");
            return sb.ToString();
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
            Uri uri = new Uri("/WinResource.xml", UriKind.RelativeOrAbsolute);
            using (Stream stream = System.Windows.Application.GetResourceStream(uri).Stream)
            {
                xmlScript.Load(stream);
            }

            //Add StringBuilder Lines to XML
            XmlDocumentFragment fragment = xmlScript.CreateDocumentFragment();
            fragment.InnerXml = stringBuilder.ToString();
            xmlScript.FirstChild.AppendChild(fragment);

            return xmlScript;
        }
        #endregion
    }
}
