using System.Collections.Generic;
using System.Geometries.Algorithm;
using System.Geometries.Algorithm.Locate;
using System.Geometries.Utilities;

namespace System.Geometries.Operation.Predicate
{
    /// <summary>I
    /// Implementation of the <tt>Intersects</tt> spatial predicate
    /// optimized for the case where one <see cref="IGeometry"/> is a rectangle. 
    /// </summary>
    /// <remarks>
    /// This class works for all input geometries, including <see cref="IGeometryCollection"/>s.
    /// <para/>
    /// As a further optimization, this class can be used in batch style
    /// to test many geometries against a single rectangle.
    /// </remarks>
    public class RectangleIntersects
    {
        /// <summary>     
        /// Crossover size at which brute-force intersection scanning
        /// is slower than indexed intersection detection.
        /// Must be determined empirically.  Should err on the
        /// safe side by making value smaller rather than larger.
        /// </summary>
        public const int MaximumScanSegmentCount = 200;

        /// <summary>
        /// Tests whether a rectangle intersects a given geometry.
        /// </summary>
        /// <param name="rectangle">A rectangular polygon</param>
        /// <param name="b">A geometry of any kind</param>
        /// <returns><c>true</c> if the geometries intersect.</returns>
        public static bool Intersects(IPolygon rectangle, IGeometry b)
        {
            var rp = new RectangleIntersects(rectangle);
            return rp.Intersects(b);
        }

        private readonly IPolygon _rectangle;
        private readonly IEnvelope _rectEnv;

        /// <summary>
        /// Create a new intersects computer for a rectangle.
        /// </summary>
        /// <param name="rectangle">A rectangular polygon.</param>
        public RectangleIntersects(IPolygon rectangle)
        {
            _rectangle = rectangle;
            _rectEnv = rectangle.GetBounds();
        }

        /// <summary>
        /// Tests whether the given Geometry intersects the query rectangle.
        /// </summary>
        /// <param name="geom">The Geometry to test (may be of any type)</param>
        /// <returns><value>true</value> if an intersection must occur 
        /// or <value>false</value> if no conclusion about intersection can be made</returns>
        public bool Intersects(IGeometry geom)
        {
            if (!_rectEnv.Intersects(geom.GetBounds()))
                return false;

            /**
             * Test if rectangle envelope intersects any component envelope.
             * This handles Point components as well
             */
            var visitor = new EnvelopeIntersectsVisitor(_rectEnv);
            visitor.ApplyTo(geom);
            if (visitor.Intersects)
                return true;

            /**
             * Test if any rectangle vertex is contained in the target geometry
             */
            var ecpVisitor = new GeometryContainsPointVisitor(_rectangle);
            ecpVisitor.ApplyTo(geom);
            if (ecpVisitor.ContainsPoint)
                return true;

            /**
             * Test if any target geometry line segment intersects the rectangle
             */
            var riVisitor = new RectangleIntersectsSegmentVisitor(_rectangle);
            riVisitor.ApplyTo(geom);
            return riVisitor.Intersects;
        }
    }

    /// <summary>
    /// Tests whether it can be concluded that a rectangle intersects a geometry,
    /// based on the relationship of the envelope(s) of the geometry.
    /// </summary>
    /// <author>Martin Davis</author>
    internal class EnvelopeIntersectsVisitor : ShortCircuitedGeometryVisitor
    {
        private readonly IEnvelope _rectEnv;

        /// <summary>
        /// Creates an instance of this class using the provided <c>Envelope</c>
        /// </summary>
        /// <param name="rectEnv">The query envelope</param>
        public EnvelopeIntersectsVisitor(IEnvelope rectEnv)
        {
            _rectEnv = rectEnv;
        }

        /// <summary>
        /// Reports whether it can be concluded that an intersection occurs, 
        /// or whether further testing is required.
        /// </summary>
        /// <returns><c>true</c> if an intersection must occur <br/> 
        /// or <c>false</c> if no conclusion about intersection can be made</returns>
        public bool Intersects { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        protected override void Visit(IGeometry element)
        {
            IEnvelope elementEnv = element.GetBounds();

            // disjoint => no intersection
            if (!_rectEnv.Intersects(elementEnv))
                return;

            // rectangle contains target env => must intersect
            if (_rectEnv.Contains(elementEnv))
            {
                Intersects = true;
                return;
            }
            /*
             * Since the envelopes intersect and the test element is connected,
             * if its envelope is completely bisected by an edge of the rectangle
             * the element and the rectangle must touch. (This is basically an application of
             * the Jordan Curve Theorem). The alternative situation is that the test
             * envelope is "on a corner" of the rectangle envelope, i.e. is not
             * completely bisected. In this case it is not possible to make a conclusion
             */
            if (elementEnv.Min.X >= _rectEnv.Min.X && elementEnv.Max.X <= _rectEnv.Max.X)
            {
                Intersects = true;
                return;
            }
            if (elementEnv.Min.Y >= _rectEnv.Min.Y && elementEnv.Max.Y <= _rectEnv.Max.Y)
            {
                Intersects = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool IsDone()
        {
            return Intersects;
        }
    }

    /// <summary>
    /// A visitor which tests whether it can be 
    /// concluded that a geometry contains a vertex of
    /// a query geometry.
    /// </summary>
    /// <author>Martin Davis</author>
    internal class GeometryContainsPointVisitor : ShortCircuitedGeometryVisitor
    {
        private readonly ICoordinateCollection _rectSeq;
        private readonly IEnvelope _rectEnv;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        public GeometryContainsPointVisitor(IPolygon rectangle)
        {
            _rectSeq = rectangle.ExteriorRing.Coordinates;
            _rectEnv = rectangle.GetBounds();
        }

        /// <summary>
        /// Gets a value indicating whether it can be concluded that a corner point of the rectangle is
        /// contained in the geometry, or whether further testing is required.
        /// </summary>
        /// <returns><value>true</value> if a corner point is contained 
        /// or <value>false</value> if no conclusion about intersection can be made
        /// </returns>
        public bool ContainsPoint { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        protected override void Visit(IGeometry geom)
        {
            if (!(geom is IPolygon))
                return;

            IEnvelope elementEnv = geom.GetBounds();

            if (!_rectEnv.Intersects(elementEnv))
                return;

            // test each corner of rectangle for inclusion
            ICoordinate rectPt;

            for (var i = 0; i < 4; i++)
            {
                rectPt = _rectSeq.Get(i);

                if (!elementEnv.Contains(rectPt))
                    continue;

                // check rect point in poly (rect is known not to touch polygon at this point)
                if (SimplePointInAreaLocator.ContainsPointInPolygon(rectPt, (IPolygon)geom))
                {
                    ContainsPoint = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool IsDone()
        {
            return ContainsPoint;
        }
    }

    /// <summary>
    /// A visitor to test for intersection between the query rectangle and the line segments of the geometry.
    /// </summary>
    /// <author>Martin Davis</author>
    internal class RectangleIntersectsSegmentVisitor : ShortCircuitedGeometryVisitor
    {
        private readonly IEnvelope _rectEnv;
        private readonly RectangleLineIntersector _rectIntersector;

        private ICoordinate _p0;
        private ICoordinate _p1;

        /// <summary>
        /// Creates a visitor for checking rectangle intersection with segments
        /// </summary>
        /// <param name="rectangle">the query rectangle </param>
        public RectangleIntersectsSegmentVisitor(IPolygon rectangle)
        {
            _rectEnv = rectangle.GetBounds();
            _rectIntersector = new RectangleLineIntersector(_rectEnv);
        }

        /// <summary>Reports whether any segment intersection exists.</summary>
        /// <returns>true if a segment intersection exists or
        /// false if no segment intersection exists</returns>
        public bool Intersects { get; private set; }

        protected override void Visit(IGeometry geom)
        {
            /**
             * It may be the case that the rectangle and the 
             * envelope of the geometry component are disjoint,
             * so it is worth checking this simple condition.
             */
            var elementEnv = geom.GetBounds();
            if (!_rectEnv.Intersects(elementEnv))
                return;

            // check segment intersections
            // get all lines from geometry component
            // (there may be more than one if it's a multi-ring polygon)
            var lines = LinearComponentExtracter.GetLines(geom);
            CheckIntersectionWithLineStrings(lines);
        }

        private void CheckIntersectionWithLineStrings(IEnumerable<IGeometry> lines)
        {
            foreach (ILineString testLine in lines)
            {
                CheckIntersectionWithSegments(testLine);
                if (Intersects)
                    return;
            }
        }

        private void CheckIntersectionWithSegments(ICurve testLine)
        {
            ICoordinateCollection seq1 = testLine.Coordinates;

            for (var j = 1; j < seq1.Count; j++)
            {
                _p0 = seq1.Get(j - 1);
                _p1 = seq1.Get(j);

                if (!_rectIntersector.Intersects(_p0, _p1)) continue;
                Intersects = true;
                return;
            }
        }

        protected override bool IsDone()
        {
            return Intersects;
        }
    }
}
