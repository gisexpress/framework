using System.Collections.Generic;
using System.Geometries.Graph;

namespace System.Geometries.Operation.Overlay
{
    /// <summary>
    /// A ring of edges which may contain nodes of degree > 2.
    /// A MaximalEdgeRing may represent two different spatial entities:
    /// a single polygon possibly containing inversions (if the ring is oriented CW)
    /// a single hole possibly containing exversions (if the ring is oriented CCW)    
    /// If the MaximalEdgeRing represents a polygon,
    /// the interior of the polygon is strongly connected.
    /// These are the form of rings used to define polygons under some spatial data models.
    /// However, under the OGC SFS model, MinimalEdgeRings are required.
    /// A MaximalEdgeRing can be converted to a list of MinimalEdgeRings using the
    /// <c>BuildMinimalRings()</c> method.
    /// </summary>
    internal class MaximalEdgeRing : EdgeRing
    {
        public MaximalEdgeRing(DirectedEdge start)
            : base(start)
        {
        }

        public override DirectedEdge GetNext(DirectedEdge de)
        {
            return de.Next;
        }

        public override void SetEdgeRing(DirectedEdge de, EdgeRing er)
        {
            de.EdgeRing = er;
        }

        /// <summary> 
        /// For all nodes in this EdgeRing,
        /// link the DirectedEdges at the node to form minimalEdgeRings
        /// </summary>
        public void LinkDirectedEdgesForMinimalEdgeRings()
        {
            DirectedEdge de = StartEdge;

            do
            {
                Node node = de.Node;
                ((DirectedEdgeStar)node.Edges).LinkMinimalDirectedEdges(this);
                de = de.Next;
            }
            while (de != StartEdge);
        }

        public IList<EdgeRing> BuildMinimalRings()
        {
            IList<EdgeRing> minEdgeRings = new List<EdgeRing>();
            DirectedEdge de = StartEdge;

            do
            {
                if (de.MinEdgeRing == null)
                {
                    EdgeRing minEr = new MinimalEdgeRing(de);
                    minEdgeRings.Add(minEr);
                }

                de = de.Next;
            }
            while (de != StartEdge);

            return minEdgeRings;
        }
    }
}
