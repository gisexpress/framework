using System.Diagnostics;

namespace System.Geometries.Graph
{
    abstract internal class GraphComponent
    {
        protected GraphComponent()
        {
        }

        protected GraphComponent(Label label)
        {
            Label = label;
        }

        protected bool iCovered;
        protected bool iCoveredSet;

        public Label Label;
        public bool InResult;
        
        public bool IsInResult
        {
            get { return InResult; }
        }

        public bool Covered
        {
            get
            {
                return iCovered;
            }
            set
            {
                iCovered = value;
                iCoveredSet = true;
            }
        }

        public bool IsCovered
        {
            get { return Covered; }
        }

        public bool IsCoveredSet
        {
            get { return iCoveredSet; }
        }

        public bool Visited;

        public bool IsVisited
        {
            get { return Visited; }
        }

        /// <returns>
        /// A coordinate in this component (or null, if there are none).
        /// </returns>
        public abstract ICoordinate Coordinate
        {
            get;
            protected set;
        }

        /// <summary>
        /// Compute the contribution to an IM for this component.
        /// </summary>
        public abstract void ComputeIM(IntersectionMatrix im);

        /// <summary>
        /// An isolated component is one that does not intersect or touch any other
        /// component.  This is the case if the label has valid locations for
        /// only a single Geometry.
        /// </summary>
        /// <returns><c>true</c> if this component is isolated.</returns>
        public abstract bool IsIsolated
        {
            get;
        }

        /// <summary>
        /// Update the IM with the contribution for this component.
        /// A component only contributes if it has a labelling for both parent geometries.
        /// </summary>
        /// <param name="im"></param>
        public void UpdateIM(IntersectionMatrix im)
        {
            Debug.Assert(Label.GeometryCount >= 2, "found partial label");
            ComputeIM(im);
        }
    }
}
