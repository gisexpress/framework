using System;
using System.Geometries;
using System.Geometries.Utilities;

namespace System.Geometries.Index.Strtree
{
    /// <summary>
    /// A pair of <see cref="IBoundable{Envelope, TItem}"/>s, whose leaf items 
    /// support a distance metric between them.
    /// Used to compute the distance between the members,
    /// and to expand a member relative to the other
    /// in order to produce new branches of the 
    /// Branch-and-Bound evaluation tree.
    /// Provides an ordering based on the distance between the members,
    /// which allows building a priority queue by minimum distance.
    /// </summary>
    /// <author>Martin Davis</author>
    internal class BoundablePair<TItem> : IComparable<BoundablePair<TItem>>
    {
        private readonly IBoundable<IEnvelope, TItem> _boundable1;
        private readonly IBoundable<IEnvelope, TItem> _boundable2;
        private readonly double _distance;
        private readonly IItemDistance<IEnvelope, TItem> _itemDistance;
        //private double _maxDistance = -1.0;

        /// <summary>
        /// Creates an instance of this class with the given <see cref="IBoundable{Envelope, TItem}"/>s and the <see cref="IItemDistance{Envelope, TItem}"/> function.
        /// </summary>
        /// <param name="boundable1">The first boundable</param>
        /// <param name="boundable2">The second boundable</param>
        /// <param name="itemDistance">The item distance function</param>
        public BoundablePair(IBoundable<IEnvelope, TItem> boundable1, IBoundable<IEnvelope, TItem> boundable2, IItemDistance<IEnvelope, TItem> itemDistance)
        {
            _boundable1 = boundable1;
            _boundable2 = boundable2;
            _itemDistance = itemDistance;
            _distance = GetDistance();
        }

        /// <summary>
        /// Gets one of the member <see cref="IBoundable{IEnvelope, TItem}"/>s in the pair 
        /// (indexed by [0, 1]).
        /// </summary>
        /// <param name="i">The index of the member to return (0 or 1)</param>
        /// <returns>The chosen member</returns>
        public IBoundable<IEnvelope, TItem> GetBoundable(int i)
        {
            return i == 0 ? _boundable1 : _boundable2;
        }

        /// <summary>
        /// Computes the distance between the <see cref="IBoundable{IEnvelope, TItem}"/>s in this pair.
        /// The boundables are either composites or leaves.
        /// If either is composite, the distance is computed as the minimum distance
        /// between the bounds.  
        /// If both are leaves, the distance is computed by <see cref="IItemDistance{IEnvelope, TItem}.Distance(IBoundable{IEnvelope, TItem}, IBoundable{IEnvelope, TItem})"/>.
        /// </summary>
        /// <returns>The distance between the <see cref="IBoundable{IEnvelope, TItem}"/>s in this pair.</returns>
        private double GetDistance()
        {
            // if items, compute exact distance
            if (IsLeaves)
            {
                return _itemDistance.Distance(_boundable1, _boundable2);
            }
            // otherwise compute distance between bounds of boundables
            return _boundable1.Bounds.Distance(_boundable2.Bounds);
        }

        /// <summary>
        /// Gets the minimum possible distance between the Boundables in
        /// this pair. 
        /// If the members are both items, this will be the
        /// exact distance between them.
        /// Otherwise, this distance will be a lower bound on 
        /// the distances between the items in the members.
        /// </summary>
        /// <returns>The exact or lower bound distance for this pair</returns>
        public double Distance
        {
            get { return _distance; }
        }

        /// <summary>
        /// Compares two pairs based on their minimum distances
        /// </summary>
        public int CompareTo(BoundablePair<TItem> o)
        {
            if (_distance < o._distance) return -1;
            if (_distance > o._distance) return 1;
            return 0;
        }

        /// <summary>
        /// Tests if both elements of the pair are leaf nodes
        /// </summary>
        public bool IsLeaves
        {
            get { return !(IsComposite(_boundable1) || IsComposite(_boundable2)); }
        }

        public static bool IsComposite(IBoundable<IEnvelope, TItem> item)
        {
            return (item is AbstractNode<IEnvelope, TItem>);
        }

        /// <summary>
        /// For a pair which is not a leaf 
        /// (i.e. has at least one composite boundable)
        /// computes a list of new pairs 
        /// from the expansion of the larger boundable.
        /// </summary>
        public void ExpandToQueue(PriorityQueue<BoundablePair<TItem>> priQ, double minDistance)
        {
            bool isComp1 = IsComposite(_boundable1);
            bool isComp2 = IsComposite(_boundable2);

            /**
             * HEURISTIC: If both boundable are composite,
             * choose the one with largest area to expand.
             * Otherwise, simply expand whichever is composite.
             */
            if (isComp1 && isComp2)
            {
                if (_boundable1.Bounds.GetArea() > _boundable2.Bounds.GetArea())
                {
                    Expand(_boundable1, _boundable2, priQ, minDistance);
                    return;
                }
                Expand(_boundable2, _boundable1, priQ, minDistance);
                return;
            }
            if (isComp1)
            {
                Expand(_boundable1, _boundable2, priQ, minDistance);
                return;
            }
            if (isComp2)
            {
                Expand(_boundable2, _boundable1, priQ, minDistance);
                return;
            }

            throw new ArgumentException("neither boundable is composite");
        }

        private void Expand(IBoundable<IEnvelope, TItem> bndComposite, IBoundable<IEnvelope, TItem> bndOther, PriorityQueue<BoundablePair<TItem>> priQ, double minDistance)
        {
            var children = ((AbstractNode<IEnvelope, TItem>)bndComposite).ChildBoundables;

            foreach (var child in children)
            {
                var bp = new BoundablePair<TItem>(child, bndOther, _itemDistance);
                // only add to queue if this pair might contain the closest points
                // MD - it's actually faster to construct the object rather than called distance(child, bndOther)!
                if (bp.Distance < minDistance)
                {
                    priQ.Add(bp);
                }
            }
        }
    }
}