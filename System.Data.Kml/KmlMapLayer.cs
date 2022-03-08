using System.Collections.Generic;
using System.CoordinateSystems;
using System.Geometries;
using System.Linq;
using System.Runtime.InteropServices;
using System.Workspace;
using System.Xml;

namespace System.Data.Kml
{
    [Guid("A88BCE83-B764-46D9-A909-69F5F55A3F7A")]
    internal class KmlMapLayer : MapLayer
    {
        public KmlMapLayer(XmlDocument document) : base(string.Empty, Constants.Xml.Folder, Constants.Xml.NamespaceURI, document)
        {
            Crs = CoordinateSystemFactory.Create(4326);
        }

        KmlPlacemarkCollection Items;
        KmlPlacemarkCollection GetPlacemarks()
        {
            return Items ?? (Items = new KmlPlacemarkCollection(OwnerDocument) { ParentNode = this });
        }

        public override bool FeatureSupport
        {
            get { return true; }
        }

        public override bool StyleSupport
        {
            get { return true; }
        }

        protected override ICoordinateSystem OnFindCoordinateSystem()
        {
            return Crs;
        }

        public override IFeature FindFeature(object featureId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IFeature> GetFeatures(IEnvelope bounds)
        {
            if (bounds == null)
            {
                return GetPlacemarks();
            }

            return GetPlacemarks().Where(e => bounds.Intersects(e.GetBounds()));
        }

        public override IFeature NewFeature()
        {
            return new KmlPlacemark(OwnerDocument) { Command = this };
        }

        public override IEnvelope GetBounds()
        {
            var e = default(IEnvelope);

            foreach (IGeometry g in GetFeatures().Select(feature => feature.GetGeometry()))
            {
                if (g == null)
                {
                    continue;
                }

                if (e == null)
                {
                    e = g.GetBounds();
                }
                else
                {
                    e.ExpandToInclude(g.GetBounds());
                }
            }

            if (e == null)
            {
                return e;
            }

            return e.Transform(Transform);
        }
    }
}
