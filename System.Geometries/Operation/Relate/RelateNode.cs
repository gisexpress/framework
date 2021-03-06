using System.Geometries.Graph;

namespace System.Geometries.Operation.Relate
{
    /// <summary>
    /// A RelateNode is a Node that maintains a list of EdgeStubs
    /// for the edges that are incident on it.
    /// </summary>
    internal class RelateNode : Node
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="edges"></param>
        public RelateNode(ICoordinate coord, EdgeEndStar edges) :
            base(coord, edges) { }

        /// <summary>
        /// Update the IM with the contribution for this component.
        /// A component only contributes if it has a labelling for both parent geometries.
        /// </summary>
        public override void ComputeIM(IntersectionMatrix im)
        {
            im.SetAtLeastIfValid(Label.GetLocation(0), Label.GetLocation(1), Dimensions.Point);
        }

        /// <summary>
        /// Update the IM with the contribution for the EdgeEnds incident on this node.
        /// </summary>
        /// <param name="im"></param>
        public void UpdateIMFromEdges(IntersectionMatrix im)
        {
            ((EdgeEndBundleStar) Edges).UpdateIM(im);
        }
    }
}
