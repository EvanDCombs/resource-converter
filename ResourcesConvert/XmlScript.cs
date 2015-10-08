using System.Xml;
using System.Xml.XPath;

namespace ResourcesConvert
{
    public interface IXmlScript
    {
        void XmlWrite(XmlScript xmlScript, XmlElement parent);
        object XmlParse(XPathNavigator xPathNavigator);
        //T XmlParse<T>(XPathNavigator xPathNavigator);
    }
	public class XmlScript : XmlDocument
    {
		public XmlScript () : base()
		{
			XmlDeclaration declaration = CreateXmlDeclaration ("1.0", "UTF-8", null);
			XmlElement root = DocumentElement;
			InsertBefore (declaration, root);
		}

		#region Creating Xml
		public static XmlElement XmlRoot(XmlDocument xmlDocument, string tag)
		{
			XmlElement element = xmlDocument.CreateElement (tag);
			xmlDocument.AppendChild (element);
			return element;
		}
		public static XmlElement XmlElement(XmlDocument xmlDocument, XmlElement parent, string tag)
		{
			XmlElement element = xmlDocument.CreateElement (tag);
			parent.AppendChild (element);
			return element;
		}
		public static XmlText XmlText(XmlDocument xmlDocument, XmlElement parent, string text)
		{
			XmlText xmlText = xmlDocument.CreateTextNode (text);
			parent.AppendChild (xmlText);
			return xmlText;
		}
		public static void XmlAttribute(XmlElement parent, string name, string value)
		{
			parent.SetAttribute (name, value);
		}
		public static XmlElement XmlElementWithText(XmlDocument xmlDocument, XmlElement parent, string tag, string text)
		{
			XmlElement element = XmlElement (xmlDocument, parent, tag);
			XmlText (xmlDocument, element, text);
			return element;
		}
		public static XmlElement XmlElementWithAttribute(XmlDocument xmlDocument, XmlElement parent, string tag, string attribute, string value)
		{
			XmlElement element = XmlElement (xmlDocument, parent, tag);
			XmlAttribute (element, attribute, value);
			return element;
		}
        public static XmlElement XmlElementWithAttributeAndText(XmlDocument xmlDocument, XmlElement parent, string tag, string text, string attributeName, string attributeValue)
        {
            XmlElement element = XmlElement(xmlDocument, parent, tag);
            XmlAttribute(element, attributeName, attributeValue);
            XmlText(xmlDocument, element, text);
            return element;
        }
        #endregion

        #region Reading Xml
        public static string GetStringXML(XPathNavigator xPathNavigator)
		{
			if (xPathNavigator.MoveToNext ())
			{
				return xPathNavigator.Value;
			}
			return null;
		}
		#endregion
    }
}
