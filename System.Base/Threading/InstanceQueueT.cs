using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Threading
{
    /// <summary>
    /// Represents the pool for instance of the <typeparamref name="TValue"/>
    /// </summary>
    /// <typeparam name="TValue">
    /// The element type of the pool
    /// </typeparam>
    public class InstanceQueue<TValue> : IDisposable where TValue : class, ICloneable
    {
        public InstanceQueue()
            : this(default(TValue))
        {
        }

        public InstanceQueue(TValue value)
        {
            FreeItems = new Stack<TValue>();
            UsedItems = new HashSet<TValue>();

            Value = value ?? Activator.CreateInstance<TValue>();
            InitSize = DeveloperEnvironment.WebApplication ? 6 : 1;

            InstanceQueue.Current.Add(this);
        }

        ~InstanceQueue()
        {
            Clear();
        }

        public readonly TValue Value;

        protected int NumClones;
        protected readonly int InitSize;
        protected readonly Stack<TValue> FreeItems;
        protected readonly HashSet<TValue> UsedItems;

        public void Clear()
        {
            NumClones = 0;

            FreeItems.ToList().ForEach(e => e.DisposeSafely());
            UsedItems.ToList().ForEach(e => e.DisposeSafely());

            FreeItems.Clear();
            UsedItems.Clear();
        }

        /// <summary>
        /// Returns an thread-safe instance of the <typeparamref name="TValue"/> from pool for temporary use.
        /// The instance that given should added to pool again by Push method.
        /// </summary>
        /// <returns>
        /// An thread-safe instance of the <typeparamref name="TValue"/>
        /// </returns>
        public TValue Pop()
        {
            TValue value;

            lock (FreeItems)
            {
                if (FreeItems.Count == 0)
                {
                    Expand();
                }

                value = FreeItems.Pop();

                if (UsedItems.Add(value) == false)
                {
                    throw new InvalidOperationException("{0} instance is already in use".FormatInvariant(typeof(TValue).Name));
                }
            }

            return value;
        }

        /// <summary>
        /// Adds the instance of the <typeparamref name="TValue"/> to the pool again.
        /// </summary>
        /// <param name="value">
        /// An instance of the <typeparamref name="TValue"/>
        /// </param>
        public void Push(TValue value)
        {
            if (value == null)
            {
                return;
            }

            lock (FreeItems)
            {
                if (UsedItems.Remove(value))
                {
                    FreeItems.Push(value);
                }
                else
                {
                    throw new InvalidOperationException("{0} instance does not belong to the pool".FormatInvariant(typeof(TValue).Name));
                }
            }
        }

        void Expand()
        {
            lock (Value)
            {
                for (int n = 0; n < InitSize; n++)
                {
                    NumClones++;
                    FreeItems.Push((TValue)Value.Clone());
                }

                if ((NumClones - InitSize) > (FreeItems.Count + UsedItems.Count))
                {
                    throw new InvalidOperationException("{0} instance pool validation failed".FormatInvariant(typeof(TValue).Name));
                }
            }
        }

        public void Dispose()
        {
            Clear();
            Value.DisposeSafely();
            GC.SuppressFinalize(this);
        }
    }
}
