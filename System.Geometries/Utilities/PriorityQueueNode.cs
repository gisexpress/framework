namespace System.Geometries.Utilities
{
    /// <summary>
    /// A container for a prioritized node that sites in an
    /// <see cref="AlternativePriorityQueue{TPriority, TData}"/>.
    /// </summary>
    /// <typeparam name="TPriority">
    /// The type to use for the priority of the node in the queue.
    /// </typeparam>
    /// <typeparam name="TData">
    /// The type to use for the data stored by the node in the queue.
    /// </typeparam>
    public sealed class PriorityQueueNode<TPriority, TData>
    {
        private readonly TData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueueNode{TPriority, TData}"/> class.
        /// </summary>
        /// <param name="data">
        /// The <typeparamref name="TData"/> to store in this node.
        /// </param>
        public PriorityQueueNode(TData data)
        {
            this.data = data;
        }

        internal PriorityQueueNode(PriorityQueueNode<TPriority, TData> copyFrom)
        {
            this.data = copyFrom.data;
            this.Priority = copyFrom.Priority;
            this.QueueIndex = copyFrom.QueueIndex;
        }

        /// <summary>
        /// Gets the <typeparamref name="TData"/> that is stored in this node.
        /// </summary>
        public TData Data { get { return this.data; } }

        /// <summary>
        /// Gets the <typeparamref name="TPriority"/> of this node in the queue.
        /// </summary>
        /// <remarks>
        /// The queue may update this priority while the node is still in the queue.
        /// </remarks>
        public TPriority Priority { get; internal set; }

        /// <summary>
        /// Gets or sets the index of this node in the queue.
        /// </summary>
        /// <remarks>
        /// This should only be read and written by the queue itself.
        /// It has no "real" meaning to anyone else.
        /// </remarks>
        internal int QueueIndex { get; set; }
    }
}
