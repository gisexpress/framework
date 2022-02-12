using System.Collections.Generic;
using System.Diagnostics;
using System.Geometries.Algorithm;

namespace System.Geometries.Graph
{
    internal abstract class EdgeRing
    {
        protected EdgeRing(DirectedEdge start)
        {
            Holes = new List<EdgeRing>();
            Points = new List<ICoordinate>();
            EdgeList = new List<DirectedEdge>();
            LabelNull = new Label(Locations.Null);

            if (IsValid = ComputePoints(start))
            {
                ComputeRing();
            }
        }

        protected int NodeDegree = -1;

        protected EdgeRing ShellRing;
        protected DirectedEdge StartEdge;
        protected readonly Label LabelNull;

        protected readonly List<EdgeRing> Holes;
        protected readonly List<ICoordinate> Points;
        protected readonly List<DirectedEdge> EdgeList;

        public bool IsValid
        {
            get;
            protected set;
        }

        public bool IsIsolated
        {
            get
            {
                return LabelNull.GeometryCount == 1;
            }
        }

        public bool IsHole
        {
            get;
            protected set;
        }

        public ICoordinate GetCoordinate(int i)
        {
            return Points[i];
        }

        public ILinearRing LinearRing
        {
            get;
            protected set;
        }

        public Label Label
        {
            get
            {
                return LabelNull;
            }
        }

        public bool IsShell
        {
            get { return ShellRing == null; }
        }

        public EdgeRing Shell
        {
            get
            {
                return ShellRing;
            }
            set
            {
                ShellRing = value;

                if (value != null)
                {
                    ShellRing.AddHole(this);
                }
            }
        }

        public void AddHole(EdgeRing ring)
        {
            Holes.Add(ring);
        }

        public IPolygon ToPolygon()
        {
            IPolygon value = LinearRing.Factory.Create<IPolygon>();

            value.ExteriorRing = LinearRing;
            
            for (int i = 0; i < Holes.Count; i++)
            {
                value.InteriorRings.Add(Holes[i].LinearRing);
            }

            return value;
        }

        /// <summary>
        /// Compute a LinearRing from the point list previously collected.
        /// Test if the ring is a hole (i.e. if it is CCW) and set the hole flag
        /// accordingly.
        /// </summary>
        public void ComputeRing()
        {
            if (LinearRing == null)
            {
                LinearRing = StartEdge.Edge.Sequence.Factory.Create<ILinearRing>();
                LinearRing.Coordinates.Add(Points);
                IsHole = CGAlgorithms.IsCCW(LinearRing.Coordinates);
            }
        }

        public abstract DirectedEdge GetNext(DirectedEdge de);

        public abstract void SetEdgeRing(DirectedEdge de, EdgeRing er);

        /// <summary> 
        /// Returns the list of DirectedEdges that make up this EdgeRing.
        /// </summary>
        public IList<DirectedEdge> Edges
        {
            get
            {
                return EdgeList;
            }
        }

        /// <summary> 
        /// Collect all the points from the DirectedEdges of this ring into a contiguous list.
        /// </summary>
        /// <param name="start"></param>
        protected bool ComputePoints(DirectedEdge start)
        {
            StartEdge = start;
            DirectedEdge de = start;
            bool isFirstEdge = true;

            do
            {
                if (de == null)
                {
                    // found null Directed Edge
                    return false;
                }

                if (de.EdgeRing == this)
                {
                    // "Directed Edge visited twice during ring-building at " + de.Coordinate;
                    return false;
                }

                EdgeList.Add(de);
                Label label = de.Label;
                Debug.Assert(label.IsArea());
                MergeLabel(label);
                AddPoints(de.Edge, de.IsForward, isFirstEdge);
                isFirstEdge = false;
                SetEdgeRing(de, this);
                de = GetNext(de);
            }
            while (de != StartEdge);

            return true;
        }

        public int MaxNodeDegree
        {
            get
            {
                if (NodeDegree < 0)
                {
                    ComputeMaxNodeDegree();
                }

                return NodeDegree;
            }
        }

        void ComputeMaxNodeDegree()
        {
            NodeDegree = 0;
            DirectedEdge de = StartEdge;

            do
            {
                Node node = de.Node;
                int degree = ((DirectedEdgeStar)node.Edges).GetOutgoingDegree(this);

                if (degree > NodeDegree)
                {
                    NodeDegree = degree;
                }

                de = GetNext(de);
            }
            while (de != StartEdge);

            NodeDegree *= 2;
        }

        public void SetInResult()
        {
            DirectedEdge de = StartEdge;
            do
            {
                de.Edge.InResult = true;
                de = de.Next;
            }
            while (de != StartEdge);
        }

        protected void MergeLabel(Label deLabel)
        {
            MergeLabel(deLabel, 0);
            MergeLabel(deLabel, 1);
        }

        /// <summary> 
        /// Merge the RHS label from a DirectedEdge into the label for this EdgeRing.
        /// The DirectedEdge label may be null.  This is acceptable - it results
        /// from a node which is NOT an intersection node between the Geometries
        /// (e.g. the end node of a LinearRing).  In this case the DirectedEdge label
        /// does not contribute any information to the overall labelling, and is simply skipped.
        /// </summary>
        /// <param name="deLabel"></param>
        /// <param name="geomIndex"></param>
        protected void MergeLabel(Label deLabel, int geomIndex)
        {
            Locations loc = deLabel.GetLocation(geomIndex, Positions.Right);
            // no information to be had from this label
            if (loc == Locations.Null)
                return;
            // if there is no current RHS value, set it
            if (LabelNull.GetLocation(geomIndex) == Locations.Null)
            {
                LabelNull.SetLocation(geomIndex, loc);
                return;
            }
        }

        protected void AddPoints(Edge edge, bool isForward, bool isFirstEdge)
        {
            if (isForward)
            {
                int startIndex = 1;

                if (isFirstEdge)
                {
                    startIndex = 0;
                }

                for (int i = startIndex; i < edge.Sequence.Count; i++)
                {
                    Points.Add(edge.Sequence.Get(i));
                }
            }
            else
            {
                // is backward
                int startIndex = edge.Sequence.Count - 2;

                if (isFirstEdge)
                {
                    startIndex = edge.Sequence.Count - 1;
                }

                for (int i = startIndex; i >= 0; i--)
                {
                    Points.Add(edge.Sequence.Get(i));
                }
            }
        }

        /// <summary> 
        /// This method will cause the ring to be computed.
        /// It will also check any holes, if they have been assigned.
        /// </summary>
        /// <param name="p"></param>
        public bool ContainsPoint(Coordinate p)
        {
            ILinearRing shell = LinearRing;
            IEnvelope env = shell.GetBounds();

            if (!env.Contains(p))
                return false;

            if (!CGAlgorithms.IsPointInRing(p, shell.Coordinates))
                return false;

            foreach (EdgeRing hole in Holes)
            {
                if (hole.ContainsPoint(p))
                    return false;
            }
            return true;
        }
    }
}
