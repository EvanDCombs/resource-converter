using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesConvert
{
    public class ConvertToiOS : ConvertTo
    {
        #region Properties
        protected override string ResourceFileName { get { return "//Localizable_"; } }
        protected override string CSharpFileName { get { return "Strings"; } }
        protected override string Folder { get { return "//iOS"; } }
        protected override string GetString { get { return "NSBundle.MainBundle.LocalizedString(\"name\", null);"; } }
        protected override string ResourceFileExtention { get { return STRINGS_EXTENSION; } }
        protected override StringBuilder UsingStatements { get { return new StringBuilder("using Foundation"); } }
        #endregion
        #region Initialization
        private static ConvertToiOS instance;
        public static ConvertToiOS Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConvertToiOS(Resource.Properties);
                }
                return instance;
            }
        }
        private ConvertToiOS(PropertyInfo[] properties) : base(properties){}
        #endregion
        #region Methods
        protected override string GetResourceString(string key, string value)
        {
            return "\"" + key + "\"=\"" + value + "\";";
        }
        #endregion
    }
}
