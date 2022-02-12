﻿//////////////////////////////////////////////////////////////////////////////////////////////////
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
using System.Collections.Specialized;
using System.Linq;

namespace System.Collections
{
    public class NameObjectCollection : NameObjectCollection<object>
    {
    }

    public class NameObjectCollection<T> : NameObjectCollectionBase, IEnumerable<T>
    {
        public NameObjectCollection()
        {
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Add(string name, T value)
        {
            InsertItem(name, value);
        }

        public bool Contains(string name)
        {
            return BaseGet(name).HasValue();
        }

        public string GetKey(int index)
        {
            return BaseGetKey(index);
        }

        public virtual T this[int index]
        {
            get { return (T)BaseGet(index); }
        }

        public T this[string name]
        {
            get { return (T)BaseGet(name); }
        }

        public bool TryGetValue(string name, out T value)
        {
            value = (T)BaseGet(name);
            return value.HasValue();
        }

        protected virtual void InsertItem(string name, T item)
        {
            BaseAdd(name, item);
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return BaseGetAllValues().Cast<T>().GetEnumerator();
        }
    }
}