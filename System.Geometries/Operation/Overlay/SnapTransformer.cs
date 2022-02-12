using System.Geometries.Utilities;

namespace System.Geometries.Operation.Overlay
{
    internal class SnapTransformer : GeometryTransformer
    {
        public SnapTransformer(double tolerance, ICoordinate[] points)
        {
            SnapTolerance = tolerance;
            SnapPts = points;
        }

        public SnapTransformer(double tolerance, ICoordinate[] points, bool isSelfSnap)
            : this(tolerance, points)
        {
            IsSelfSnap = isSelfSnap;
        }

        readonly bool IsSelfSnap;
        readonly double SnapTolerance;
        readonly ICoordinate[] SnapPts;

        protected override ICoordinateCollection TransformCoordinates(ICoordinateCollection coords, IGeometry parent)
        {
            var c = coords.Factory.Create<ICoordinateCollection>();
            c.Add(SnapLine(coords, SnapPts));
            return c;
        }

        ICoordinate[] SnapLine(ICoordinateCollection src, ICoordinate[] snaps)
        {
            return new LineStringSnapper(src, SnapTolerance) { AllowSnappingToSourceVertices = IsSelfSnap }.SnapTo(snaps);
        }
    }
}
