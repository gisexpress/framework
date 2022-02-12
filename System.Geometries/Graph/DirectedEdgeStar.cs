using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace System.Geometries.Graph
{
    /// <summary> 
    /// A DirectedEdgeStar is an ordered list of outgoing DirectedEdges around a node.
    /// It supports labelling the edges as well as linking the edges to form both
    /// MaximalEdgeRings and MinimalEdgeRings.
    /// </summary>
    internal class DirectedEdgeStar : EdgeEndStar
    {
        /// <summary> 
        /// A list of all outgoing edges in the result, in CCW order.
        /// </summary>
        protected IList<DirectedEdge> AreaEdgeList;

        /// <summary> 
        /// Insert a directed edge in the list.
        /// </summary>
        /// <param name="ee"></param>
        public override void Insert(EdgeEnd ee)
        {
            DirectedEdge de = (DirectedEdge)ee;
            InsertEdgeEnd(de, de);
        }

        public Label Label;

        public int GetOutgoingDegree()
        {
            int degree = 0;

            foreach (DirectedEdge e in Edges)
            {
                if (e.IsInResult)
                {
                    degree++;
                }
            }

            return degree;
        }

        public int GetOutgoingDegree(EdgeRing er)
        {
            int degree = 0;

            foreach (DirectedEdge de in Edges)
            {
                if (de.EdgeRing == er)
                {
                    degree++;
                }
            }

            return degree;
        }

        public DirectedEdge GetRightmostEdge()
        {
            IList<EdgeEnd> edges = Edges;
            int size = edges.Count;

            if (size < 1)
            {
                return default(DirectedEdge);
            }

            var de0 = (DirectedEdge)edges[0];

            if (size == 1)
            {
                return de0;
            }

            var deLast = (DirectedEdge)edges[size - 1];

            int quad0 = de0.Quadrant;
            int quad1 = deLast.Quadrant;

            if (QuadrantOp.IsNorthern(quad0) && QuadrantOp.IsNorthern(quad1))
            {
                return de0;
            }
            else if (!QuadrantOp.IsNorthern(quad0) && !QuadrantOp.IsNorthern(quad1))
            {
                return deLast;
            }
            else
            {
                // edges are in different hemispheres - make sure we return one that is non-horizontal                        
                if (de0.Dy != 0)
                {
                    return de0;
                }
                else if (deLast.Dy != 0)
                {
                    return deLast;
                }
            }

            Debug.Fail("found two horizontal edges incident on node");
            return default(DirectedEdge);
        }

        /// <summary> 
        /// Compute the labelling for all dirEdges in this star, as well
        /// as the overall labelling.
        /// </summary>
        /// <param name="geom"></param>
        public override bool ComputeLabelling(GeometryGraph[] geom)
        {
            if (base.ComputeLabelling(geom))
            {
                Label = new Label(Locations.Null);
                IEnumerator<EdgeEnd> it = GetEnumerator();

                while (it.MoveNext())
                {
                    EdgeEnd ee = it.Current;
                    Edge e = ee.Edge;
                    Label eLabel = e.Label;

                    for (int i = 0; i < 2; i++)
                    {
                        Locations eLoc = eLabel.GetLocation(i);

                        if (eLoc == Locations.Interior || eLoc == Locations.Boundary)
                        {
                            Label.SetLocation(i, Locations.Interior);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary> 
        /// For each dirEdge in the star, merge the label .
        /// </summary>
        public void MergeSymLabels()
        {
            foreach (DirectedEdge de in Edges)
            {
                Label label = de.Label;
                label.Merge(de.Directed.Label);
            }
        }

        /// <summary> 
        /// Update incomplete dirEdge labels from the labelling for the node.
        /// </summary>
        /// <param name="nodeLabel"></param>
        public void UpdateLabelling(Label nodeLabel)
        {
            foreach (DirectedEdge de in Edges)
            {
                Label label = de.Label;
                label.SetAllLocationsIfNull(0, nodeLabel.GetLocation(0));
                label.SetAllLocationsIfNull(1, nodeLabel.GetLocation(1));
            }
        }

        IList<DirectedEdge> GetResultAreaEdges()
        {
            if (AreaEdgeList == null)
            {
                AreaEdgeList = new List<DirectedEdge>();

                foreach (DirectedEdge de in Edges)
                {
                    if (de.IsInResult || de.Directed.IsInResult)
                    {
                        AreaEdgeList.Add(de);
                    }
                }
            }

            return AreaEdgeList;
        }

        const int ScanningForIncoming = 1;
        const int LinkingToOutgoing = 2;

        /// <summary> 
        /// Traverse the star of DirectedEdges, linking the included edges together.
        /// To link two dirEdges, the next pointer for an incoming dirEdge
        /// is set to the next outgoing edge.
        /// DirEdges are only linked if:
        /// they belong to an area (i.e. they have sides)
        /// they are marked as being in the result
        /// Edges are linked in CCW order (the order they are stored).
        /// This means that rings have their face on the Right
        /// (in other words, the topological location of the face is given by the RHS label of the DirectedEdge).
        /// PRECONDITION: No pair of dirEdges are both marked as being in the result.
        /// </summary>
        public bool LinkResultDirectedEdges()
        {
            // make sure edges are copied to resultAreaEdges list
            GetResultAreaEdges();

            // find first area edge (if any) to start linking at
            DirectedEdge firstOut = null;
            DirectedEdge incoming = null;
            int state = ScanningForIncoming;

            // link edges in CCW order
            for (int i = 0; i < AreaEdgeList.Count; i++)
            {
                DirectedEdge nextOut = AreaEdgeList[i];
                DirectedEdge nextIn = nextOut.Directed;

                // skip de's that we're not interested in
                if (!nextOut.Label.IsArea())
                {
                    continue;
                }

                // record first outgoing edge, in order to link the last incoming edge
                if (firstOut == null && nextOut.IsInResult)
                {
                    firstOut = nextOut;
                }

                switch (state)
                {
                    case ScanningForIncoming:
                        if (!nextIn.IsInResult) continue;
                        incoming = nextIn;
                        state = LinkingToOutgoing;
                        break;
                    case LinkingToOutgoing:
                        if (!nextOut.IsInResult) continue;
                        incoming.Next = nextOut;
                        state = ScanningForIncoming;
                        break;
                    default:
                        break;
                }
            }

            if (state == LinkingToOutgoing)
            {
                if (firstOut == null)
                {
                    Debug.Fail(string.Concat("No outgoing dirEdge found at ", Coordinate));
                    return false;
                }

                if (firstOut.IsInResult)
                {
                    incoming.Next = firstOut;
                }
                else
                {
                    Debug.Fail("Unable to link last incoming dirEdge");
                    return false;
                }
            }

            return true;
        }

        public void LinkMinimalDirectedEdges(EdgeRing er)
        {
            // find first area edge (if any) to start linking at
            DirectedEdge firstOut = null;
            DirectedEdge incoming = null;
            int state = ScanningForIncoming;

            // link edges in CW order
            for (int i = AreaEdgeList.Count - 1; i >= 0; i--)
            {
                DirectedEdge nextOut = this.AreaEdgeList[i];
                DirectedEdge nextIn = nextOut.Directed;

                // record first outgoing edge, in order to link the last incoming edge
                if (firstOut == null && nextOut.EdgeRing == er)
                {
                    firstOut = nextOut;
                }

                switch (state)
                {
                    case ScanningForIncoming:
                        if (nextIn.EdgeRing != er)
                            continue;
                        incoming = nextIn;
                        state = LinkingToOutgoing;
                        break;
                    case LinkingToOutgoing:
                        if (nextOut.EdgeRing != er)
                            continue;
                        incoming.NextMin = nextOut;
                        state = ScanningForIncoming;
                        break;
                    default:
                        break;
                }
            }

            if (state == LinkingToOutgoing)
            {
                Debug.Assert(firstOut != null, "found null for first outgoing dirEdge");
                Debug.Assert(firstOut.EdgeRing == er, "unable to link last incoming dirEdge");
                incoming.NextMin = firstOut;
            }
        }

        public void LinkAllDirectedEdges()
        {
            IList<EdgeEnd> temp = Edges;
            temp = null;    //Hack
            // find first area edge (if any) to start linking at
            DirectedEdge prevOut = null;
            DirectedEdge firstIn = null;

            // link edges in CW order
            for (int i = edgeList.Count - 1; i >= 0; i--)
            {
                DirectedEdge nextOut = (DirectedEdge)edgeList[i];
                DirectedEdge nextIn = nextOut.Directed;
                if (firstIn == null)
                    firstIn = nextIn;
                if (prevOut != null)
                    nextIn.Next = prevOut;
                // record outgoing edge, in order to link the last incoming edge
                prevOut = nextOut;
            }

            firstIn.Next = prevOut;
        }

        /// <summary> 
        /// Traverse the star of edges, maintaing the current location in the result
        /// area at this node (if any).
        /// If any L edges are found in the interior of the result, mark them as covered.
        /// </summary>
        public void FindCoveredLineEdges()
        {
            // Since edges are stored in CCW order around the node,
            // as we move around the ring we move from the right to the left side of the edge

            /*
            * Find first DirectedEdge of result area (if any).
            * The interior of the result is on the RHS of the edge,
            * so the start location will be:
            * - Interior if the edge is outgoing
            * - Exterior if the edge is incoming
            */
            Locations startLoc = Locations.Null;

            foreach (DirectedEdge nextOut in Edges)
            {
                DirectedEdge nextIn = nextOut.Directed;

                if (!nextOut.IsLineEdge)
                {
                    if (nextOut.IsInResult)
                    {
                        startLoc = Locations.Interior;
                        break;
                    }

                    if (nextIn.IsInResult)
                    {
                        startLoc = Locations.Exterior;
                        break;
                    }
                }
            }

            // no A edges found, so can't determine if Curve edges are covered or not
            if (startLoc == Locations.Null)
            {
                return;
            }

            /*
            * move around ring, keeping track of the current location
            * (Interior or Exterior) for the result area.
            * If Curve edges are found, mark them as covered if they are in the interior
            */
            Locations currLoc = startLoc;

            foreach (DirectedEdge nextOut in Edges)
            {
                DirectedEdge nextIn = nextOut.Directed;
                if (nextOut.IsLineEdge)
                    nextOut.Edge.Covered = (currLoc == Locations.Interior);
                else
                {
                    // edge is an Area edge
                    if (nextOut.IsInResult)
                        currLoc = Locations.Exterior;
                    if (nextIn.IsInResult)
                        currLoc = Locations.Interior;
                }
            }
        }

        public bool ComputeDepths(DirectedEdge de)
        {
            int edgeIndex = FindIndex(de);
            int startDepth = de.GetDepth(Positions.Left);
            int targetLastDepth = de.GetDepth(Positions.Right);
            // compute the depths from this edge up to the end of the edge array
            int nextDepth = ComputeDepths(edgeIndex + 1, edgeList.Count, startDepth);
            // compute the depths for the initial part of the array
            int lastDepth = ComputeDepths(0, edgeIndex, nextDepth);

            if (lastDepth == targetLastDepth)
            {
                return true;
            }

            // Fail Depth mismatch at de.Coordinate
            return false;
        }

        /// <summary> 
        /// Compute the DirectedEdge depths for a subsequence of the edge array.
        /// </summary>
        /// <returns>The last depth assigned (from the R side of the last edge visited).</returns>
        int ComputeDepths(int startIndex, int endIndex, int startDepth)
        {
            int currDepth = startDepth;

            for (int i = startIndex; i < endIndex; i++)
            {
                DirectedEdge nextDe = (DirectedEdge)edgeList[i];

                if (nextDe.SetEdgeDepths(Positions.Right, currDepth))
                {
                    currDepth = nextDe.GetDepth(Positions.Left);
                }
            }

            return currDepth;
        }

        public override void Write(StreamWriter outstream)
        {
            foreach (DirectedEdge de in Edges)
            {
                outstream.Write("out ");
                de.Write(outstream);
                outstream.WriteLine();
                outstream.Write("in ");
                de.Directed.Write(outstream);
                outstream.WriteLine();
            }
        }
    }
}
