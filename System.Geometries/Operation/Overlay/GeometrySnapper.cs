using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Geometries.Operation.Overlay
{
    internal class GeometrySnapper
    {
        public const double SnapPrexisionFactor = 1E-9;

        /// <summary>
        /// Creates a new snapper acting on the given geometry
        /// </summary>
        /// <param name="g">the geometry to snap</param>
        public GeometrySnapper(IGeometry g)
        {
            Source = g;
        }

        readonly IGeometry Source;

        /// <summary>
        /// Estimates the snap tolerance for a Geometry, taking into account its precision model.
        /// </summary>
        /// <param name="g"></param>
        /// <returns>The estimated snap tolerance</returns>
        public static double ComputeOverlaySnapTolerance(IGeometry g)
        {
            return ComputeSizeBasedSnapTolerance(g.GetBounds());
        }

        public static double ComputeSizeBasedSnapTolerance(IEnvelope e)
        {
            return Math.Min(e.GetHeight(), e.GetWidth()) * SnapPrexisionFactor;
        }

        public static double ComputeOverlaySnapTolerance(IGeometry g0, IGeometry g1)
        {
            return Math.Min(ComputeOverlaySnapTolerance(g0), ComputeOverlaySnapTolerance(g1));
        }

        /// <summary>
        /// Snaps a geometry to itself.
        /// Allows optionally cleaning the result to ensure it is topologically valid
        /// (which fixes issues such as topology collapses in polygonal inputs).
        /// Snapping a geometry to itself can remove artifacts such as very narrow slivers, gores and spikes.
        /// </summary>
        /// <param name="g">the geometry to snap</param>
        /// <param name="tolerance">the snapping tolerance</param>
        /// <param name="clean">whether the result should be made valid</param>
        /// <returns>a new snapped <see cref="Geometry"/></returns>
        public static IGeometry SnapToSelf(IGeometry g, double tolerance, bool clean)
        {
            return new GeometrySnapper(g).SnapToSelf(tolerance, clean);
        }

        /// <summary>
        ///  Snaps the vertices in the component <see cref="ILineString" />s
        ///  of the source geometry to the vertices of the given snap geometry.
        /// </summary>
        /// <param name="g">a geometry to snap the source to</param>
        /// <param name="tolerance"></param>
        /// <returns>a new snapped Geometry</returns>
        public IGeometry SnapTo(IGeometry g, double tolerance)
        {
            return new SnapTransformer(tolerance, ExtractTargetCoordinates(g)).Transform(Source);
        }

        /// Snaps the vertices in the component <see cref="ILineString" />s
        /// of the source geometry to the vertices of the same geometry.
        /// Allows optionally cleaning the result to ensure it is topologically valid
        /// (which fixes issues such as topology collapses in polygonal inputs).
        /// <param name="tolerance">The snapping tolerance</param>
        /// <param name="clean">Whether the result should be made valid</param>
        /// <returns>The geometry snapped to itself</returns>
        public IGeometry SnapToSelf(double tolerance, bool clean)
        {
            IGeometry snapped = new SnapTransformer(tolerance, ExtractTargetCoordinates(Source), true).Transform(Source);

            if (clean && snapped is IPolygonal)
            {
                return snapped.Buffer(tolerance);
            }

            return snapped;
        }

        Coordinate[] ExtractTargetCoordinates(IGeometry g)
        {
            // TODO: should do this more efficiently.  Use CoordSeq filter to get points, KDTree for uniqueness & queries
            var ptSet = new HashSet<Coordinate>(g.Coordinates);
            var result = new Coordinate[ptSet.Count];
            ptSet.CopyTo(result, 0);
            Array.Sort(result);
            return result;
        }

        /// <summary>
        /// Computes the snap tolerance based on the input geometries.
        /// </summary>
        static double ComputeSnapTolerance(Coordinate[] ringPoints)
        {
            // Use a small percentage of this to be safe
            return ComputeMinimumSegmentLength(ringPoints) / 10.0;
        }

        static double ComputeMinimumSegmentLength(Coordinate[] points)
        {
            var minSegLen = Double.MaxValue;

            for (var i = 0; i < points.Length - 1; i++)
            {
                double length = points[i].Distance(points[i + 1]);

                if (length < minSegLen)
                {
                    minSegLen = length;
                }
            }

            return minSegLen;
        }
    }
}
