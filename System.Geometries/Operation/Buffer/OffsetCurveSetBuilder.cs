using System.Collections.Generic;
using System.ComponentModel;
using System.Geometries.Algorithm;
using System.Geometries.Graph;
using System.Geometries.Noding;

namespace System.Geometries.Operation.Buffer
{
    /// <summary>
    /// Creates all the raw offset curves for a buffer of a <c>Geometry</c>.
    /// Raw curves need to be noded together and polygonized to form the final buffer area.
    /// </summary>
    internal class OffsetCurveSetBuilder
    {
        public OffsetCurveSetBuilder(Geometry inputGeom, double distance, OffsetCurveBuilder curveBuilder)
        {
            Input = inputGeom;
            Distance = distance;
            CurveBuilder = curveBuilder;
        }

        readonly Geometry Input;
        readonly double Distance;
        readonly OffsetCurveBuilder CurveBuilder;
        readonly IList<ISegmentString> Curves = new List<ISegmentString>();

        /// <summary>
        /// Computes the set of raw offset curves for the buffer.
        /// Each offset curve has an attached {Label} indicating
        /// its left and right location.
        /// </summary>
        /// <returns>A Collection of SegmentStrings representing the raw buffer curves.</returns>
        public IList<ISegmentString> GetCurves()
        {
            Add(Input);
            return Curves;
        }

        /// <summary>
        /// Creates a {SegmentString} for a coordinate list which is a raw offset curve,
        /// and adds it to the list of buffer curves.
        /// The SegmentString is tagged with a Label giving the topology of the curve.
        /// The curve may be oriented in either direction.
        /// If the curve is oriented CW, the locations will be:
        /// Left: Location.Exterior.
        /// Right: Location.Interior.
        /// </summary>
        void AddCurve(ICoordinate[] points, Locations leftLoc, Locations rightLoc)
        {
            // Don't add null or trivial curves!
            if (points == null || points.Length < 2)
            {
                return;
            }

            // Add the edge for a coordinate list which is a raw offset curve
            Curves.Add(new NodedSegmentString(Input.Factory.Create<ICoordinateCollection>(points), new Label(0, Locations.Boundary, leftLoc, rightLoc)));
        }

        void Add(IGeometry g)
        {
            if (g is IPolygon)
            {
                AddPolygon((IPolygon)g);
            }
            else if (g is ILineString)
            {
                AddLineString(g);
            }
            else if (g is IPoint)
            {
                AddPoint(g);
            }
            else if (g is IGeometryCollection)
            {
                AddCollection(g);
            }
            else throw new NotSupportedException(g.GetType().FullName);
        }

        void AddCollection(IGeometry g)
        {
            for (var i = 0; i < g.NumGeometries(); i++)
            {
                Add(g.GetGeometryN(i));
            }
        }

        /// <summary>
        /// Add a Point to the graph.
        /// </summary>
        void AddPoint(IGeometry g)
        {
            // a zero or negative width buffer of a line/point is empty
            if (Distance <= 0.0)
            {
                return;
            }

            ICoordinate[] coord = g.Coordinates.ToArray();
            ICoordinate[] curve = CurveBuilder.GetLineCurve(coord, Distance);

            AddCurve(curve, Locations.Exterior, Locations.Interior);
        }

        void AddLineString(IGeometry line)
        {
            // A zero or negative width buffer of a line/point is empty
            if (Distance <= 0.0 && !CurveBuilder.BufferParameters.SingleSide)
            {
                return;
            }

            ICoordinate[] coord = line.Coordinates.ToArray();
            ICoordinate[] curve = CurveBuilder.GetLineCurve(coord, Distance);

            AddCurve(curve, Locations.Exterior, Locations.Interior);
        }

        void AddPolygon(IPolygon p)
        {
            var offsetDistance = Distance;
            var offsetSide = Positions.Left;

            if (Distance < 0.0)
            {
                offsetDistance = -Distance;
                offsetSide = Positions.Right;
            }

            ILinearRing shell = p.ExteriorRing;
            ICoordinate[] shellCoord = shell.Coordinates.ToArray();

            // optimization - don't bother computing buffer
            // if the polygon would be completely eroded
            if (Distance < 0.0 && IsErodedCompletely(shellCoord, Distance))
            {
                return;
            }

            // don't attemtp to buffer a polygon with too few distinct vertices
            if (Distance <= 0.0 && shellCoord.Length < 3)
            {
                return;
            }

            AddPolygonRing(shellCoord, offsetDistance, offsetSide, Locations.Exterior, Locations.Interior);

            for (var i = 0; i < p.InteriorRings.Count; i++)
            {
                ILinearRing hole = p.InteriorRings.Get(i);
                ICoordinate[] holeCoord = hole.Coordinates.ToArray();

                // optimization - don't bother computing buffer for this hole
                // if the hole would be completely covered
                if (Distance > 0.0 && IsErodedCompletely(holeCoord, -Distance))
                {
                    continue;
                }

                // Holes are topologically labelled opposite to the shell, since
                // the interior of the polygon lies on their opposite side
                // (on the left, if the hole is oriented CCW)
                AddPolygonRing(holeCoord, offsetDistance, Position.Opposite(offsetSide), Locations.Interior, Locations.Exterior);
            }
        }

        /// <summary>
        /// Adds an offset curve for a polygon ring.
        /// The side and left and right topological location arguments
        /// assume that the ring is oriented CW.
        /// If the ring is in the opposite orientation,
        /// the left and right locations must be interchanged and the side flipped.
        /// </summary>
        /// <param name="coord">The coordinates of the ring (must not contain repeated points).</param>
        /// <param name="offsetDistance">The distance at which to create the buffer.</param>
        /// <param name="side">The side of the ring on which to construct the buffer line.</param>
        /// <param name="cwLeftLoc">The location on the L side of the ring (if it is CW).</param>
        /// <param name="cwRightLoc">The location on the R side of the ring (if it is CW).</param>
        void AddPolygonRing(ICoordinate[] coord, double offsetDistance, Positions side, Locations cwLeftLoc, Locations cwRightLoc)
        {
            // don't bother adding ring if it is "flat" and will disappear in the output
            if (offsetDistance == 0.0 && coord.Length < LinearRing.MinimumValidSize)
                return;

            var leftLoc = cwLeftLoc;
            var rightLoc = cwRightLoc;

            if (coord.Length >= LinearRing.MinimumValidSize && CGAlgorithms.IsCCW(coord))
            {
                leftLoc = cwRightLoc;
                rightLoc = cwLeftLoc;
                side = Position.Opposite(side);
            }

            var curve = CurveBuilder.GetRingCurve(coord, side, offsetDistance);
            AddCurve(curve, leftLoc, rightLoc);
        }

        /// <summary>
        /// The ringCoord is assumed to contain no repeated points.
        /// It may be degenerate (i.e. contain only 1, 2, or 3 points).
        /// In this case it has no area, and hence has a minimum diameter of 0.
        /// </summary>
        /// <param name="ringCoord"></param>
        /// <param name="bufferDistance"></param>
        /// <returns></returns>
        bool IsErodedCompletely(ICoordinate[] ringCoord, double bufferDistance)
        {
            // degenerate ring has no area
            if (ringCoord.Length < 4)
                return bufferDistance < 0;

            // important test to eliminate inverted triangle bug
            // also optimizes erosion test for triangles
            if (ringCoord.Length == 4)
                return IsTriangleErodedCompletely(ringCoord, bufferDistance);

            /*
             * The following is a heuristic test to determine whether an
             * inside buffer will be eroded completely.
             * It is based on the fact that the minimum diameter of the ring pointset
             * provides an upper bound on the buffer distance which would erode the
             * ring.
             * If the buffer distance is less than the minimum diameter, the ring
             * may still be eroded, but this will be determined by
             * a full topological computation.
             *
             */
            var ring = Input.Factory.Create<ILinearRing>();
            ring.Coordinates.Add(ringCoord);
            var md = new MinimumDiameter(ring);
            double minDiam = md.Length;
            return minDiam < 2 * Math.Abs(bufferDistance);
        }

        /// <summary>
        /// Tests whether a triangular ring would be eroded completely by the given
        /// buffer distance.
        /// This is a precise test.  It uses the fact that the inner buffer of a
        /// triangle converges on the inCentre of the triangle (the point
        /// equidistant from all sides).  If the buffer distance is greater than the
        /// distance of the inCentre from a side, the triangle will be eroded completely.
        /// This test is important, since it removes a problematic case where
        /// the buffer distance is slightly larger than the inCentre distance.
        /// In this case the triangle buffer curve "inverts" with incorrect topology,
        /// producing an incorrect hole in the buffer.
        /// </summary>
        /// <param name="triangleCoord"></param>
        /// <param name="bufferDistance"></param>
        /// <returns></returns>
        static bool IsTriangleErodedCompletely(ICoordinate[] triangleCoord, double bufferDistance)
        {
            var tri = new Triangle(triangleCoord[0], triangleCoord[1], triangleCoord[2]);
            var inCentre = tri.InCentre;
            var distToCentre = CGAlgorithms.DistancePointLine(inCentre, tri.P0, tri.P1);
            return distToCentre < Math.Abs(bufferDistance);
        }
    }
}