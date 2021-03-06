//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright © GISExpress 2015-2022. All Rights Reserved.
//  
//  GISExpress .NET API and Component Library
//  
//  The entire contents of this file is protected by local and International Copyright Laws.
//  Unauthorized reproduction, reverse-engineering, and distribution of all or any portion of
//  the code contained in this file is strictly prohibited and may result in severe civil and 
//  criminal penalties and will be prosecuted to the maximum extent possible under the law.
//  
//  RESTRICTIONS
//  
//  THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES ARE CONFIDENTIAL AND PROPRIETARY TRADE SECRETS OF GISExpress
//  THE REGISTERED DEVELOPER IS LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET COMPONENTS AS PART OF AN EXECUTABLE PROGRAM ONLY.
//  
//  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE
//  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT
//  AND PERMISSION FROM GISExpress
//  
//  CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.
//  
//  Warning: This content was generated by GISExpress tools.
//  Changes to this content may cause incorrect behavior and will be lost if the content is regenerated.
//
///////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace System.Collections
{
    public class SafeSortedList<TKey, TValue> : IList, IEnumerable<TValue>, IDisposable
    {
        public SafeSortedList()
        {
            Items = new Dictionary<TKey, TValue>();
        }

        protected readonly Dictionary<TKey, TValue> Items;

        public int Count
        {
            get { return Items.Count; }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public void Add(TKey key, TValue value)
        {
            OnInsertItem(key, value, false);
        }

        public bool Get(TKey key, out TValue value, Func<object, TValue> valueGet, object argument)
        {
            if (!Items.TryGetValue(key, out value))
            {
                lock (Items)
                {
                    if (!Items.TryGetValue(key, out value))
                    {
                        OnInsertItem(key, value = valueGet(argument), true);
                        return false;
                    }
                }
            }

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Items.TryGetValue(key, out value);
        }

        public bool Remove(TKey key)
        {
            TValue value;

            if (TryGetValue(key, out value))
            {
                return OnRemoveItem(key, value);
            }

            return false;
        }

        public virtual IEnumerator<TValue> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        protected virtual void OnInsertItem(TKey key, TValue value, bool isMember)
        {
            Items.Add(key, value);
        }

        protected virtual bool OnRemoveItem(TKey key, TValue value)
        {
            return Items.Remove(key);
        }

        #region IList

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            Items.Clear();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool IList.IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public virtual void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }
    }
}
