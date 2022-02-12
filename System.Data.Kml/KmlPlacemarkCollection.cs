using System.Xml;

namespace System.Data.Kml
{
    class KmlPlacemarkCollection : XmlFeatureElementCollection<KmlPlacemark>
    {
        public KmlPlacemarkCollection(XmlDocument document) : base(document)
        {
        }
    }
}
