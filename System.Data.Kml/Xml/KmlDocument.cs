using System.Xml;

namespace System.Data.Kml
{
    class KmlDocument : XmlDocumentBase
    {
        public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
        {
            switch (localName)
            {
                case Constants.Xml.Placemark:
                    return new KmlPlacemark(prefix, localName, namespaceURI, this);

                case Constants.Xml.Folder:
                case Constants.Xml.Document:
                    return new KmlFolder(prefix, localName, namespaceURI, this);
            }

            return new KmlElement(prefix, localName, namespaceURI, this);
        }
    }
}
