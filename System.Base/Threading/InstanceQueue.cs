using System.Collections.Generic;

namespace System.Threading
{
    public class InstanceQueue : IDisposable
    {
        static InstanceQueue()
        {
            Current = new InstanceQueue();
        }

        internal InstanceQueue()
        {
            Items = new List<IDisposable>();
        }

        public static readonly InstanceQueue Current;

        protected readonly List<IDisposable> Items;

        internal void Add(IDisposable value)
        {
            Items.Add(value);
        }

        public void Dispose()
        {
            while (Items.Count > 0)
            {
                Items[0].Dispose();
                Items.RemoveAt(0);
            }

            GC.SuppressFinalize(this);
        }
    }
}
