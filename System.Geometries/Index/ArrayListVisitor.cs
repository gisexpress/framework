using System.Collections.Generic;

namespace System.Geometries.Index
{
    public class ArrayListVisitor : ArrayListVisitor<object>
    {
    }

    public class ArrayListVisitor<T> : IItemVisitor<T>
    {
        private readonly List<T> _items = new List<T>();
        
        ///// <summary>
        ///// 
        ///// </summary>
        //public ArrayListVisitor() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void VisitItem(T item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<T> Items
        {
            get
            {
                return _items;
            }
        }
    }
}
