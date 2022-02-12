using System.IO;

namespace System.Geometries.Graph
{
    internal class DirectedEdge : EdgeEnd
    {
        /// <summary>
        /// Computes the factor for the change in depth when moving from one location to another.
        /// E.g. if crossing from the Interior to the Exterior the depth decreases, so the factor is -1.
        /// </summary>
        public static int DepthFactor(Locations currLocation, Locations nextLocation)
        {
            if (currLocation == Locations.Exterior && nextLocation == Locations.Interior)
            {
                return 1;
            }
            else if (currLocation == Locations.Interior && nextLocation == Locations.Exterior)
            {
                return -1;
            }

            return 0;
        }

        /// <summary> 
        /// The depth of each side (position) of this edge.
        /// The 0 element of the array is never used.
        /// </summary>
        protected readonly int[] Depth = { 0, -999, -999 };

        public DirectedEdge(Edge edge, bool forward)
            : base(edge)
        {
            IsForward = forward;

            if (forward)
            {
                Init(edge.Sequence.Get(0), edge.Sequence.Get(1));
            }
            else
            {
                int n = edge.NumPoints - 1;
                Init(edge.Sequence.Get(n), edge.Sequence.Get(n - 1));
            }

            if (IsValid)
            {
                ComputeDirectedLabel();
            }
        }

        public bool InResult
        {
            get;
            set;
        }

        public bool IsInResult
        {
            get { return InResult; }
        }

        public bool Visited
        {
            get;
            set;
        }

        public bool IsVisited
        {
            get { return Visited; }
        }

        public EdgeRing EdgeRing
        {
            get;
            set;
        }

        public EdgeRing MinEdgeRing
        {
            get;
            set;
        }

        public int GetDepth(Positions position)
        {
            return Depth[(int)position];
        }

        public bool SetDepth(Positions position, int depthVal)
        {
            var i = (int)position;

            if (Depth[i] != -999 && Depth[i] != depthVal)
            {
                // Fail : "Assigned depths do not match"
                return false;
            }

            Depth[i] = depthVal;
            return true;
        }

        public int DepthDelta
        {
            get
            {
                int delta = Edge.DepthDelta;

                if (IsForward)
                {
                    return delta;
                }

                return -delta;
            }
        }

        /// <summary>
        /// VisitedEdge get property returns <c>true</c> if bot Visited 
        /// and Sym.Visited are <c>true</c>.
        /// VisitedEdge set property marks both DirectedEdges attached to a given Edge.
        /// This is used for edges corresponding to lines, which will only
        /// appear oriented in a single direction in the result.
        /// </summary>
        public bool VisitedEdge
        {
            get
            {
                return Visited && Directed.Visited;
            }
            set
            {
                Visited = value;
                Directed.Visited = value;
            }
        }

        public bool IsForward
        {
            get;
            protected set;
        }

        public DirectedEdge Directed
        {
            get;
            set;
        }

        public DirectedEdge Next
        {
            get;
            set;
        }

        public DirectedEdge NextMin
        {
            get;
            set;
        }

        /// <summary>
        /// This edge is a line edge if
        /// at least one of the labels is a line label
        /// any labels which are not line labels have all Location = Exterior.
        /// </summary>
        public bool IsLineEdge
        {
            get
            {
                bool isLine = Label.IsLine(0) || Label.IsLine(1);
                bool isExteriorIfArea0 =
                    !Label.IsArea(0) || Label.AllPositionsEqual(0, Locations.Exterior);
                bool isExteriorIfArea1 =
                    !Label.IsArea(1) || Label.AllPositionsEqual(1, Locations.Exterior);
                return isLine && isExteriorIfArea0 && isExteriorIfArea1;
            }
        }

        /// <summary> 
        /// This is an interior Area edge if
        /// its label is an Area label for both Geometries
        /// and for each Geometry both sides are in the interior.
        /// </summary>
        /// <returns><c>true</c> if this is an interior Area edge.</returns>
        public bool IsInteriorAreaEdge
        {
            get
            {
                bool isInteriorAreaEdge = true;
                for (int i = 0; i < 2; i++)
                {
                    if (!(Label.IsArea(i)
                        && Label.GetLocation(i, Positions.Left) == Locations.Interior
                        && Label.GetLocation(i, Positions.Right) == Locations.Interior))
                    {
                        isInteriorAreaEdge = false;
                    }
                }
                return isInteriorAreaEdge;
            }
        }

        /// <summary>
        /// Compute the label in the appropriate orientation for this DirEdge.
        /// </summary>
        void ComputeDirectedLabel()
        {
            Label = new Label(Edge.Label);

            if (IsForward)
            {
                return;
            }

            Label.Flip();
        }

        /// <summary> 
        /// Set both edge depths.  
        /// One depth for a given side is provided.  
        /// The other is computed depending on the Location 
        /// transition and the depthDelta of the edge.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="position"></param>
        public bool SetEdgeDepths(Positions position, int depth)
        {
            // get the depth transition delta from R to Curve for this directed Edge
            int depthDelta = DepthDelta;

            // if moving from Curve to R instead of R to Curve must change sign of delta
            int directionFactor = 1;

            if (position == Positions.Left)
            {
                directionFactor = -1;
            }

            Positions oppositePos = Position.Opposite(position);

            int delta = depthDelta * directionFactor;
            int oppositeDepth = depth + delta;

            return SetDepth(position, depth) && SetDepth(oppositePos, oppositeDepth);
        }
    }
}
