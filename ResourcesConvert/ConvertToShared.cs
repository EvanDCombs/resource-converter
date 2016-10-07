using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesConvert
{
    public class ConvertToShared : ConvertTo
    {
        #region Properties
        protected override string ResourceFileName { get { return "//"; } }
        protected override string CSharpFileName { get { return "//Strings"; } }
        protected override string Folder { get { return "//Shared"; } }
        protected override string GetString { get { return "GetStringPartial(name, ref value"; } }
        protected override string ResourceFileExtention { get { return ""; } }
        protected override StringBuilder UsingStatements { get { return new StringBuilder("using System;"); } }
        #endregion
        #region Initialization
        private static ConvertToShared instance;
        public static ConvertToShared Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConvertToShared(Resource.Properties);
                }
                return instance;
            }
        }
        private ConvertToShared(PropertyInfo[] properties) : base(properties){ }
        #endregion
        #region Methods
        protected override StringBuilder CreateCSharpFile(List<Dictionary<string, string>> resources, string nameSpace)
        {
            StringBuilder sb = new StringBuilder();

            Uri uri = new Uri("csharp_strings_shared.txt", UriKind.RelativeOrAbsolute);
            using (Stream stream = System.Windows.Application.GetResourceStream(uri).Stream)
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    sb.Append(streamReader.ReadToEnd());
                }
            }

            sb.Replace(NAMESPACE, nameSpace);
            sb.Replace(USING_STATEMENTS, UsingStatements.ToString());

            StringBuilder csharp = new StringBuilder();
            int indent = 0;
            foreach (Dictionary<string, string> dictionary in resources)
            {
                string key = dictionary[PROPERTY_NAME];
                csharp.AppendLine(Indent(indent) + "public static string " + key);
                indent = 2;
                csharp.AppendLine(Indent(indent) + "{");
                csharp.AppendLine(Indent(indent + 1) + "get");
                csharp.AppendLine(Indent(indent + 1) + "{");
                csharp.AppendLine(Indent(indent + 2) + "string value = \"\";");
                csharp.AppendLine(Indent(indent + 2) + "GetStringPartial(\"" + key + "\", ref value);");
                csharp.AppendLine(Indent(indent + 2) + "return value;");
                csharp.AppendLine(Indent(indent + 1) + "}");
                csharp.AppendLine(Indent(indent) + "}");
            }
            sb.Replace(CSHARP, csharp.ToString());

            return sb;
        }
        protected override string GetResourceString(string key, string value)
        {
            return "";
        }
        #endregion
    }
}
