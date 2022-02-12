using System.IO;

namespace System.Geometries.Graph
{
    internal class Node : GraphComponent
    {
        public Node(ICoordinate c, EdgeEndStar edges)
        {
            Coordinate = c;
            Edges = edges;
            Label = new Label(0, Locations.Null);
        }

        public override ICoordinate Coordinate
        {
            get;
            protected set;
        }

        public EdgeEndStar Edges
        {
            get;
            protected set;
        }

        /// <summary>
        /// Tests whether any incident edge is flagged as
        /// being in the result.
        /// This test can be used to determine if the node is in the result,
        /// since if any incident edge is in the result, the node must be in the result as well.
        /// </summary>
        /// <returns><value>true</value> if any indicident edge in the in the result
        /// </returns>
        public bool IsIncidentEdgeInResult()
        {
            foreach (EdgeEnd e in Edges.Edges)
            {
                if (e.Edge.IsInResult)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool IsIsolated
        {
            get { return Label.GeometryCount == 1; }
        }

        public override void ComputeIM(IntersectionMatrix im)
        {
        }

        /// <summary> 
        /// Add the edge to the list of edges at this node.
        /// </summary>
        public void Add(EdgeEnd e)
        {
            Edges.Insert(e);
            e.Node = this;
        }

        public void MergeLabel(Node n)
        {
            MergeLabel(n.Label);
        }

        /// <summary>
        /// To merge labels for two nodes, the merged location for each LabelElement is computed.
        /// The location for the corresponding node LabelElement is set to the result, as long as the location is non-null.
        /// </summary>
        public void MergeLabel(Label other)
        {
            for (int i = 0; i < 2; i++)
            {
                Locations loc = ComputeMergedLocation(other, i);
                Locations thisLoc = Label.GetLocation(i);

                if (thisLoc == Locations.Null)
                {
                    Label.SetLocation(i, loc);
                }
            }
        }

        public void SetLabel(int argIndex, Locations onLocation)
        {
            if (Label == null)
            {
                Label = new Label(argIndex, onLocation);
            }
            else
            {
                Label.SetLocation(argIndex, onLocation);
            }
        }

        /// <summary> 
        /// Updates the label of a node to BOUNDARY, obeying the mod-2 boundaryDetermination rule.
        /// </summary>
        public void SetLabelBoundary(int argIndex)
        {
            if (Label == null)
            {
                return;
            }

            // determine the current location for the point (if any)
            Locations loc = Locations.Null;

            if (Label != null)
            {
                loc = Label.GetLocation(argIndex);
            }

            // flip the loc
            Locations newLoc;

            switch (loc)
            {
                case Locations.Boundary: newLoc = Locations.Interior; break;
                case Locations.Interior: newLoc = Locations.Boundary; break;
                default: newLoc = Locations.Boundary; break;
            }

            Label.SetLocation(argIndex, newLoc);
        }

        /// <summary> 
        /// The location for a given eltIndex for a node will be one
        /// of { Null, Interior, Boundary }.
        /// A node may be on both the boundary and the interior of a point;
        /// in this case, the rule is that the node is considered to be in the boundary.
        /// The merged location is the maximum of the two input values.
        /// </summary>
        public Locations ComputeMergedLocation(Label label2, int eltIndex)
        {
            Locations loc = Label.GetLocation(eltIndex);

            if (!label2.IsNull(eltIndex))
            {
                Locations nLoc = label2.GetLocation(eltIndex);

                if (loc != Locations.Boundary)
                {
                    loc = nLoc;
                }
            }
            
            return loc;
        }

        public void Write(TextWriter outstream)
        {
            outstream.WriteLine("node " + Coordinate + " lbl: " + Label);
        }

        public override string ToString()
        {
            return Coordinate + " " + Edges;
        }
    }
}
