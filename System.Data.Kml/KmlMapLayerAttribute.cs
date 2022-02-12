using System.Drawing;
using System.Workspace;

namespace System.Data.Kml
{
    internal class KmlMapLayerAttribute : MapLayerAttribute
    {
        public override bool IsBrowsable
        {
            get { return false; }
        }

        public override Type LayerType
        {
            get { return typeof(KmlMapLayer); }
        }

        public override Image Image
        {
            get { return default(Image); }
        }

        protected override MapLayerControl OnCreateControl()
        {
            return default(MapLayerControl);
        }
    }
}
