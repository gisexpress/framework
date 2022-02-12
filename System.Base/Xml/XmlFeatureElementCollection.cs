using System.ComponentModel;

namespace System.Xml
{
    public class XmlFeatureElementCollection<T> : XmlElementCollectionBase<T> where T : XmlElementBase, IKeyedObject
    {
        public XmlFeatureElementCollection(XmlDocument document) : base(document, Constants.Xml.Placemark)
        {
        }
    }
}
