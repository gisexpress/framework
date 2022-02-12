using System.Diagnostics;
using System.Geometries.Algorithm;
using System.IO;

namespace System.Geometries.Graph
{
    /// <summary> 
    /// Models the end of an edge incident on a node.
    /// </summary>
    /// <remarks>
    /// <para>
    /// EdgeEnds have a direction determined by the direction of the ray from the initial
    /// point to the next point.
    /// </para>
    /// <para>
    /// EdgeEnds are IComparable under the ordering  "a has a greater angle with the x-axis than b".
    /// This ordering is used to sort EdgeEnds around a node.
    /// </para>
    /// </remarks>
    internal class EdgeEnd : IComparable<EdgeEnd>
    {
        public EdgeEnd(Edge edge)
            : this(edge, default)
        {
        }

        public EdgeEnd(Edge edge, Label label)
        {
            Edge = edge;
            iLabel = label;
        }

        public bool Init(ICoordinate p0, ICoordinate p1)
        {
            P0 = p0;
            P1 = p1;

            iDx = p1.X - p0.X;
            iDy = p1.Y - p0.Y;

            if (QuadrantOp.TryGetQuadrant(iDx, iDy, out iQuadrant))
            {
                return IsValid = true;
            }

            //Debug.Fail("EdgeEnd with identical endpoints found");
            return false;
        }

        public bool IsValid;

        protected Label iLabel;
        protected ICoordinate P0, P1;  // points of initial line segment
        protected double iDx, iDy;      // the direction vector for this edge from its starting point
        protected int iQuadrant;

        public Edge Edge
        {
            get;
            protected set;
        }

        public Label Label
        {
            get { return iLabel; }
            protected set { iLabel = value; }
        }

        public ICoordinate Coordinate
        {
            get { return P0; }
        }

        public ICoordinate DirectedCoordinate
        {
            get { return P1; }
        }

        public int Quadrant
        {
            get { return iQuadrant; }
        }

        public double Dx
        {
            get { return iDx; }
        }

        public double Dy
        {
            get { return iDy; }
        }

        public Node Node
        {
            get;
            set;
        }

        public int CompareTo(EdgeEnd e)
        {
            return CompareDirection(e);
        }

        /// <summary> 
        /// Implements the total order relation:
        /// a has a greater angle with the positive x-axis than b.
        /// Using the obvious algorithm of simply computing the angle is not robust,
        /// since the angle calculation is obviously susceptible to roundoff.
        /// A robust algorithm is:
        /// - first compare the quadrant.  If the quadrants
        /// are different, it it trivial to determine which vector is "greater".
        /// - if the vectors lie in the same quadrant, the computeOrientation function
        /// can be used to decide the relative orientation of the vectors.
        /// </summary>
        /// <param name="e"></param>
        public int CompareDirection(EdgeEnd e)
        {
            if (iDx == e.iDx && iDy == e.iDy)
                return 0;
            // if the rays are in different quadrants, determining the ordering is trivial
            if (iQuadrant > e.iQuadrant)
                return 1;
            if (iQuadrant < e.iQuadrant)
                return -1;
            // vectors are in the same quadrant - check relative orientation of direction vectors
            // this is > e if it is CCW of e
            return CGAlgorithms.ComputeOrientation(e.P0, e.P1, P1);
        }

        /// <summary>
        /// Subclasses should override this if they are using labels
        /// </summary>
        /// <param name="boundaryNodeRule"></param>
        public virtual void ComputeLabel(IBoundaryNodeRule boundaryNodeRule) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outstream"></param>
        public virtual void Write(StreamWriter outstream)
        {
            double angle = Math.Atan2(iDy, iDx);
            string fullname = GetType().FullName;
            int lastDotPos = fullname.LastIndexOf('.');
            string name = fullname.Substring(lastDotPos + 1);
            outstream.Write("  " + name + ": " + P0 + " - " + P1 + " " + iQuadrant + ":" + angle + "   " + iLabel);
        }

        public override String ToString()
        {
            var angle = Math.Atan2(iDy, iDx);
            var className = GetType().Name;
            //var lastDotPos = className.LastIndexOf('.');
            //var name = className.Substring(lastDotPos + 1);
            return "  " + className + ": " + P0 + " - " + P1 + " " + iQuadrant + ":" + angle + "   " + iLabel;
        }
    }
}
