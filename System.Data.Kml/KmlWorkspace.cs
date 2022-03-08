using System.CoordinateSystems;
using System.Workspace;
using System.Xml;

namespace System.Data.Kml
{
    internal class KmlWorkspace : MapWorkspace
    {
        public KmlWorkspace(XmlDocument document) : base(document)
        {
            Crs = CoordinateSystemFactory.Create(3857);
        }

        readonly ICoordinateSystem Crs;

        protected override ICoordinateSystem OnFindCoordinateSystem()
        {
            return Crs;
        }
    }
}
