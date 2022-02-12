using System.Geometries.Index;
using System.Geometries.Index.IntervalRTree;
using System.Geometries.Utilities;

namespace System.Geometries.Algorithm.Locate
{
    ///<summary>
    /// Determines the location of <see cref="Coordinate"/>s relative to
    /// a <see cref="IPolygonal"/> geometry, using indexing for efficiency.
    /// This algorithm is suitable for use in cases where
    /// many points will be tested against a given area.
    /// <para/>
    /// Thread-safe and immutable.
    ///</summary>
    /// <author>Martin Davis</author>
    internal class IndexedPointInAreaLocator : IPointOnGeometryLocator
    {
        ///<summary>
        /// Creates a new locator for a given <see cref="IGeometry"/>.
        ///</summary>
        /// <param name="g">the Geometry to locate in</param>
        public IndexedPointInAreaLocator(IGeometry g)
        {
            if (g is IPolygonal)
            {
                Index = new IntervalIndexedGeometry(g);
            }
            else throw new ArgumentException("Argument must be Polygonal");
        }

        readonly IntervalIndexedGeometry Index;

        ///<summary>
        /// Determines the <see cref="Locations"/> of a point in an areal <see cref="IGeometry"/>.
        ///</summary>
        /// <param name="c">The point to test</param>
        /// <returns>The location of the point in the geometry
        /// </returns>
        public Locations Locate(ICoordinate c)
        {
            var rcc = new RayCrossingCounter(c);
            var visitor = new SegmentVisitor(rcc);

            Index.Query(c.Y, c.Y, visitor);

            return rcc.Location;
        }

        class SegmentVisitor : IItemVisitor<LineSegment>
        {
            public SegmentVisitor(RayCrossingCounter counter)
            {
                Counter = counter;
            }

            readonly RayCrossingCounter Counter;

            public void VisitItem(LineSegment seg)
            {
                Counter.CountSegment(seg.GetCoordinate(0), seg.GetCoordinate(1));
            }
        }

        class IntervalIndexedGeometry
        {
            public IntervalIndexedGeometry(IGeometry geom)
            {
                Index = new SortedPackedIntervalRTree<LineSegment>();

                foreach (ILineString line in LinearComponentExtracter.GetLines(geom))
                {
                    AddLine(line.Coordinates);
                }
            }

            readonly SortedPackedIntervalRTree<LineSegment> Index;

            void AddLine(ICoordinateCollection c)
            {
                for (int i = 1; i < c.Count; i++)
                {
                    var seg = new LineSegment(c.Get(i - 1), c.Get(i));
                    
                    double min = Math.Min(seg.P0.Y, seg.P1.Y);
                    double max = Math.Max(seg.P0.Y, seg.P1.Y);
                    
                    Index.Insert(min, max, seg);
                }
            }

            public void Query(double min, double max, IItemVisitor<LineSegment> visitor)
            {
                Index.Query(min, max, visitor);
            }
        }

    }
}