using System.Drawing;
using System.Drawing.Drawing2D;
using System.Geometries;

namespace System.ComponentModel
{
    public interface ISupportGraphics
    {
        void Paint(Graphics g, PaintStyle style, Func<ICoordinate, PointF> transform);
        void Paint(Graphics g, PaintStyle style, Func<ICoordinate, PointF> transform, bool raiseEvents);

        void AppendToPath(Graphics g, GraphicsPath path, Func<ICoordinate, PointF> transform);
        void AppendToPath(Graphics g, GraphicsPath path, Func<ICoordinate, PointF> transform, bool raiseEvents);
    }
}
