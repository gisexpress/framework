using System.Drawing;

namespace System.Geometries.Index
{
    public class SnapObject : ISpatialObject
    {
        public SnapObject(PointVisitedEventArgs e)
            : this(e.Point, e.Value, e.IsCenter, e.IsMidPoint)
        {
        }

        public SnapObject(PointF point, ICoordinate value, bool isCenter, bool isMidPoint)
        {
            int x = point.X.Round();
            int y = point.Y.Round();

            Value = value;
            IsCenter = isCenter;
            IsMidPoint = isMidPoint;

            unchecked
            {
                if (x < 0) x = 0;
                if (y < 0) y = 0;

                x -= x % 3;
                y -= y % 3;

                HashCode = x * 10000 + y;
            }
        }

        public static SnapObject FromPoint(PointF point, ICoordinate value)
        {
            return new SnapObject(point, value, false, false);
        }

        public readonly bool IsCenter;
        public readonly bool IsMidPoint;
        public readonly ICoordinate Value;

        protected readonly int HashCode;

        public IEnvelope GetBounds()
        {
            return Value.GetBounds();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}
