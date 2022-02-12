namespace System.Xml
{
    public class XmlFolderElement : XmlElementBase
    {
        protected internal XmlFolderElement(string prefix, string localName, string namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
        }

        public new virtual string Name
        {
            get { return Get(Constants.Xml.Name, string.Empty); }
            set { Set(Constants.Xml.Name, value); }
        }

        public string Description
        {
            get { return Get(Constants.Xml.Description, string.Empty); }
            set { Set(Constants.Xml.Description, value); }
        }

        public bool Open
        {
            get { return Get(Constants.Xml.Open, true); }
            set { Set(Constants.Xml.Open, value); }
        }

        public bool Visibility
        {
            get { return Get(Constants.Xml.Visibility, true); }
            set { Set(Constants.Xml.Visibility, value); }
        }
    }
}
