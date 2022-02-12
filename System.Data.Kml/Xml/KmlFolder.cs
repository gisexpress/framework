using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace System.Data.Kml
{
    class KmlFolder : XmlElementBase
    {
        protected internal KmlFolder(string prefix, string localName, string namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
        {
            Free = true;
        }

        int? Sequence;
        internal bool Free;

        public int NextId()
        {
            if (!Sequence.HasValue && HasChildNodes)
            {
                Free = false;
                Sequence = GetPlacemarks().Max(e => e.Id);
                Free = true;
            }

            return (++Sequence).Value;
        }

        public KmlPlacemark FindById(object featureId)
        {
            return GetPlacemarks().FirstOrDefault(e => e.Id == (int)featureId);
        }

        public IEnumerable<KmlPlacemark> GetPlacemarks()
        {
            return ChildNodes.OfType<KmlPlacemark>();
        }
    }
}
