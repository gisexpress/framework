using System.Collections.Generic;
using System.Diagnostics;
using System.Geometries.Algorithm;
using System.Geometries.Algorithm.Locate;
using System.IO;
using System.Text;

namespace System.Geometries.Graph
{
    /// <summary>
    /// A EdgeEndStar is an ordered list of EdgeEnds around a node.
    /// They are maintained in CCW order (starting with the positive x-axis) around the node
    /// for efficient lookup and topology building.
    /// </summary>
    internal abstract class EdgeEndStar
    {
        /// <summary>
        /// A map which maintains the edges in sorted order around the node.
        /// </summary>
        protected IDictionary<EdgeEnd, EdgeEnd> edgeMap = new SortedDictionary<EdgeEnd, EdgeEnd>();

        /// <summary> 
        /// A list of all outgoing edges in the result, in CCW order.
        /// </summary>
        protected IList<EdgeEnd> edgeList;

        /// <summary>
        /// The location of the point for this star in Geometry i Areas.
        /// </summary>
        private readonly Locations[] _ptInAreaLocation = new[] { Locations.Null, Locations.Null };

        /// <summary> 
        /// Insert a EdgeEnd into this EdgeEndStar.
        /// </summary>
        /// <param name="e"></param>
        abstract public void Insert(EdgeEnd e);

        /// <summary> 
        /// Insert an EdgeEnd into the map, and clear the edgeList cache,
        /// since the list of edges has now changed.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="obj"></param>
        protected void InsertEdgeEnd(EdgeEnd e, EdgeEnd obj)
        {
            edgeMap[e] = obj;
            edgeList = null;    // edge list has changed - clear the cache
        }

        /// <returns>
        /// The coordinate for the node this star is based at.
        /// </returns>
        public ICoordinate Coordinate
        {
            get
            {
                IEnumerator<EdgeEnd> it = GetEnumerator();
                if (!it.MoveNext())
                    return null;
                EdgeEnd e = it.Current;
                return e.Coordinate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Degree
        {
            get
            {
                return edgeMap.Count;
            }
        }

        /// <summary>
        /// Iterator access to the ordered list of edges is optimized by
        /// copying the map collection to a list.  (This assumes that
        /// once an iterator is requested, it is likely that insertion into
        /// the map is complete).
        /// </summary>
        public IEnumerator<EdgeEnd> GetEnumerator()
        {
            return Edges.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<EdgeEnd> Edges
        {
            get
            {
                if (edgeList == null)
                    edgeList = new List<EdgeEnd>(edgeMap.Values);
                return edgeList;
            }
        }

        public EdgeEnd GetNextCW(EdgeEnd ee)
        {
            IList<EdgeEnd> temp = Edges;
            temp = null;    // Hack for calling property
            int i = edgeList.IndexOf(ee);
            int iNextCW = i - 1;
            if (i == 0)
                iNextCW = edgeList.Count - 1;
            return edgeList[iNextCW];
        }

        public virtual bool ComputeLabelling(GeometryGraph[] geomGraph)
        {
            ComputeEdgeEndLabels(geomGraph[0].BoundaryNodeRule);

            // Propagate side labels  around the edges in the star for each parent Geometry

            for (int i = 0; i < geomGraph.Length; i++)
            {
                if (PropagateSideLabels(i))
                {
                    continue;
                }

                return false;
            }

            bool[] hasDimensionalCollapseEdge = { false, false };

            foreach (EdgeEnd e in Edges)
            {
                Label label = e.Label;

                for (int geomi = 0; geomi < 2; geomi++)
                {
                    if (label.IsLine(geomi) && label.GetLocation(geomi) == Locations.Boundary)
                    {
                        hasDimensionalCollapseEdge[geomi] = true;
                    }
                }
            }

            foreach (EdgeEnd e in Edges)
            {
                Label label = e.Label;

                for (int geomi = 0; geomi < 2; geomi++)
                {
                    if (label.IsAnyNull(geomi))
                    {
                        Locations loc;

                        if (hasDimensionalCollapseEdge[geomi])
                        {
                            loc = Locations.Exterior;
                        }
                        else
                        {
                            ICoordinate p = e.Coordinate;
                            loc = GetLocation(geomi, p, geomGraph);
                        }

                        label.SetAllLocationsIfNull(geomi, loc);
                    }
                }
            }

            return true;
        }

        void ComputeEdgeEndLabels(IBoundaryNodeRule boundaryNodeRule)
        {
            // Compute edge label for each EdgeEnd
            foreach (var ee in Edges)
            {
                ee.ComputeLabel(boundaryNodeRule);
            }
        }

        Locations GetLocation(int geomIndex, ICoordinate p, GeometryGraph[] geom)
        {
            // compute location only on demand
            if (_ptInAreaLocation[geomIndex] == Locations.Null)
                _ptInAreaLocation[geomIndex] = SimplePointInAreaLocator.Locate(p, geom[geomIndex].Geometry);
            return _ptInAreaLocation[geomIndex];
        }

        public bool IsAreaLabelsConsistent(GeometryGraph geometryGraph)
        {
            ComputeEdgeEndLabels(geometryGraph.BoundaryNodeRule);
            return CheckAreaLabelsConsistent(0);
        }

        bool CheckAreaLabelsConsistent(int geomIndex)
        {
            // Since edges are stored in CCW order around the node,
            // As we move around the ring we move from the right to the left side of the edge
            IList<EdgeEnd> edges = Edges;
            // if no edges, trivially consistent
            if (edges.Count <= 0)
                return true;
            // initialize startLoc to location of last Curve side (if any)
            int lastEdgeIndex = edges.Count - 1;
            Label startLabel = edges[lastEdgeIndex].Label;
            Locations startLoc = startLabel.GetLocation(geomIndex, Positions.Left);
            Debug.Assert(startLoc != Locations.Null, "Found unlabelled area edge");

            Locations currLoc = startLoc;
            foreach (EdgeEnd e in Edges)
            {
                Label label = e.Label;
                // we assume that we are only checking a area
                Debug.Assert(label.IsArea(geomIndex), "Found non-area edge");
                Locations leftLoc = label.GetLocation(geomIndex, Positions.Left);
                Locations rightLoc = label.GetLocation(geomIndex, Positions.Right);
                // check that edge is really a boundary between inside and outside!
                if (leftLoc == rightLoc)
                    return false;
                // check side location conflict                 
                if (rightLoc != currLoc)
                    return false;
                currLoc = leftLoc;
            }
            return true;
        }

        public bool PropagateSideLabels(int geomIndex)
        {
            // Since edges are stored in CCW order around the node,
            // As we move around the ring we move from the right to the left side of the edge
            Locations startLoc = Locations.Null;

            // initialize loc to location of last Curve side (if any)
            foreach (EdgeEnd e in Edges)
            {
                Label label = e.Label;

                if (label.IsArea(geomIndex) && label.GetLocation(geomIndex, Positions.Left) != Locations.Null)
                {
                    startLoc = label.GetLocation(geomIndex, Positions.Left);
                }
            }

            // no labelled sides found, so no labels to propagate
            if (startLoc == Locations.Null)
            {
                return true;
            }

            Locations currLoc = startLoc;

            foreach (EdgeEnd e in Edges)
            {
                Label label = e.Label;

                // set null On values to be in current location
                if (label.GetLocation(geomIndex, Positions.On) == Locations.Null)
                {
                    label.SetLocation(geomIndex, Positions.On, currLoc);
                }

                // set side labels (if any)
                if (label.IsArea(geomIndex))
                {
                    Locations leftLoc = label.GetLocation(geomIndex, Positions.Left);
                    Locations rightLoc = label.GetLocation(geomIndex, Positions.Right);

                    // if there is a right location, that is the next location to propagate
                    if (rightLoc != Locations.Null)
                    {
                        if (rightLoc != currLoc)
                        {
                            return false;
                            //Debug.Fail(string.Concat("side location conflict", e.Coordinate));
                            //return false;
                        }

                        if (leftLoc == Locations.Null)
                        {
                            //Debug.Fail(string.Concat("found single null side (at ", e.Coordinate, ")"));
                            return false;
                        }

                        currLoc = leftLoc;
                    }
                    else
                    {
                        if (leftLoc == Locations.Null)
                        {
                            label.SetLocation(geomIndex, Positions.Right, currLoc);
                            label.SetLocation(geomIndex, Positions.Left, currLoc);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public int FindIndex(EdgeEnd eSearch)
        {
            GetEnumerator();   // force edgelist to be computed
            for (int i = 0; i < edgeList.Count; i++)
            {
                EdgeEnd e = edgeList[i];
                if (e == eSearch)
                    return i;
            }
            return -1;
        }

        public virtual void Write(StreamWriter outstream)
        {
            foreach (EdgeEnd e in Edges)
            {
                e.Write(outstream);
            }
        }

        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.AppendLine("EdgeEndStar:   " + Coordinate);
            foreach (var e in this)
                buf.AppendLine(e.ToString());
            return buf.ToString();
        }
    }
}
