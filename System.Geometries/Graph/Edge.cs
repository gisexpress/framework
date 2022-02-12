using System.ComponentModel;
using System.Geometries.Algorithm;
using System.Geometries.Graph.Index;

namespace System.Geometries.Graph
{
    internal class Edge : GraphComponent
    {
        public Edge(ICoordinateCollection sequence)
            : this(sequence, default(Label))
        {
        }

        public Edge(ICoordinateCollection sequence, Label label)
        {
            Isolated = true;
            Sequence = sequence;
            Label = label;
        }

        public readonly ICoordinateCollection Sequence;

        protected Depth iDepth;
        protected Envelope Bounds;
        protected MonotoneChainEdge ChainEdge;
        protected EdgeIntersectionList IntersectionList;

        /// <summary> 
        /// Updates an IM from the label for an edge.
        /// Handles edges from both L and A geometries.
        /// </summary>
        /// <param name="im"></param>
        /// <param name="label"></param>
        public static void UpdateIM(Label label, IntersectionMatrix im)
        {
            im.SetAtLeastIfValid(label.GetLocation(0, Positions.On), label.GetLocation(1, Positions.On), Dimensions.Curve);
            if (label.IsArea())
            {
                im.SetAtLeastIfValid(label.GetLocation(0, Positions.Left), label.GetLocation(1, Positions.Left), Dimensions.Surface);
                im.SetAtLeastIfValid(label.GetLocation(0, Positions.Right), label.GetLocation(1, Positions.Right), Dimensions.Surface);
            }
        }

        public int NumPoints
        {
            get { return Sequence.Count; }
        }

        public string Name
        {
            get;
            set;
        }

        public override ICoordinate Coordinate
        {
            get
            {
                if (NumPoints == 0)
                {
                    return default(Coordinate);
                }

                return Sequence.StartPoint;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        public IEnvelope Envelope
        {
            get { return Sequence.GetBounds(); }
        }

        public Depth Depth
        {
            get { return iDepth ?? (iDepth = new Depth()); }
        }

        /// <summary>
        /// The depthDelta is the change in depth as an edge is crossed from R to L.
        /// </summary>
        /// <returns>The change in depth as the edge is crossed from R to L.</returns>
        public int DepthDelta
        {
            get;
            set;
        }

        public int MaximumSegmentIndex
        {
            get { return NumPoints - 1; }
        }

        public EdgeIntersectionList EdgeIntersectionList
        {
            get { return IntersectionList ?? (IntersectionList = new EdgeIntersectionList(this)); }
        }

        public MonotoneChainEdge MonotoneChainEdge
        {
            get { return ChainEdge ?? (ChainEdge = new MonotoneChainEdge(this)); }
        }

        public bool IsClosed
        {
            get { return Sequence.IsClosed; }
        }

        /// <summary> 
        /// An Edge is collapsed if it is an Area edge and it consists of
        /// two segments which are equal and opposite (eg a zero-width V).
        /// </summary>
        public bool IsCollapsed
        {
            get
            {
                if (!Label.IsArea())
                {
                    return false;
                }

                if (NumPoints != 3)
                {
                    return false;
                }

                if (Sequence.Get(0).IsEquivalent(Sequence.Get(2)))
                {
                    return true;
                }

                return false;
            }
        }

        public Edge CollapsedEdge
        {
            get
            {
                ICoordinateCollection newPts = Sequence.Factory.Create<ICoordinateCollection>();

                newPts.Add(Sequence.Get(0));
                newPts.Add(Sequence.Get(1));

                return new Edge(newPts, Label.ToLineLabel(Label));
            }
        }

        public bool Isolated
        {
            get;
            set;
        }

        public override bool IsIsolated
        {
            get { return Isolated; }
        }

        /// <summary>
        /// Adds EdgeIntersections for one or both
        /// intersections found for a segment of an edge to the edge intersection list.
        /// </summary>
        /// <param name="li"></param>
        /// <param name="segmentIndex"></param>
        /// <param name="geomIndex"></param>
        public void AddIntersections(LineIntersector li, int segmentIndex, int geomIndex)
        {
            for (var i = 0; i < li.IntersectionNum; i++)
                AddIntersection(li, segmentIndex, geomIndex, i);
        }

        /// <summary>
        /// Add an EdgeIntersection for intersection intIndex.
        /// An intersection that falls exactly on a vertex of the edge is normalized
        /// to use the higher of the two possible segmentIndexes.
        /// </summary>
        /// <param name="li"></param>
        /// <param name="segmentIndex"></param>
        /// <param name="geomIndex"></param>
        /// <param name="intIndex"></param>
        public void AddIntersection(LineIntersector li, int segmentIndex, int geomIndex, int intIndex)
        {
            Coordinate intPt = new Coordinate(li.GetIntersection(intIndex));
            var normalizedSegmentIndex = segmentIndex;
            var dist = li.GetEdgeDistance(geomIndex, intIndex);

            // normalize the intersection point location
            var nextSegIndex = normalizedSegmentIndex + 1;

            if (nextSegIndex < NumPoints)
            {
                ICoordinate nextPt = Sequence.Get(nextSegIndex);

                // Normalize segment index if intPt falls on vertex
                // The check for point equality is 2D only - Z values are ignored
                if (intPt.IsEquivalent(nextPt))
                {
                    normalizedSegmentIndex = nextSegIndex;
                    dist = 0.0;
                }

                // Add the intersection point to edge intersection list.                
                EdgeIntersectionList.Add(intPt, normalizedSegmentIndex, dist);
            }
        }

        /// <summary>
        /// Update the IM with the contribution for this component.
        /// A component only contributes if it has a labelling for both parent geometries.
        /// </summary>
        /// <param name="im"></param>
        public override void ComputeIM(IntersectionMatrix im)
        {
            UpdateIM(Label, im);
        }

        /// <summary>
        /// Equals is defined to be:
        /// e1 equals e2
        /// iff
        /// the coordinates of e1 are the same or the reverse of the coordinates in e2.
        /// </summary>
        /// <param name="o"></param>
        public override bool Equals(object o)
        {
            var e = o as Edge;

            if (o == null)
                return false;

            return Equals(e);
        }

        /// <summary>
        /// Equals is defined to be:
        /// e1 equals e2
        /// iff
        /// the coordinates of e1 are the same or the reverse of the coordinates in e2.
        /// </summary>
        /// <param name="e"></param>
        protected bool Equals(Edge e)
        {
            if (NumPoints != e.NumPoints)
            {
                return false;
            }

            var isEqualForward = true;
            var isEqualReverse = true;
            var iRev = NumPoints;

            for (var i = 0; i < NumPoints; i++)
            {
                if (!Sequence.Get(i).IsEquivalent(e.Sequence.Get(i)))
                {
                    isEqualForward = false;
                }

                if (!Sequence.Get(i).IsEquivalent(e.Sequence.Get(--iRev)))
                {
                    isEqualReverse = false;
                }

                if (!isEqualForward && !isEqualReverse)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator ==(Edge obj1, Edge obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(Edge obj1, Edge obj2)
        {
            return !(obj1 == obj2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <returns> 
        /// <c>true</c> if the coordinate sequences of the Edges are identical.
        /// </returns>
        /// <param name="e"></param>
        public bool IsPointwiseEqual(Edge e)
        {
            if (NumPoints != e.NumPoints)
            {
                return false;
            }

            for (var i = 0; i < NumPoints; i++)
            {
                if (!Sequence.Get(i).IsEquivalent(e.Sequence.Get(i)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
