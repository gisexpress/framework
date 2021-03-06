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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.CoordinateSystems.Epsg
{
    internal class EpsgTableCollection : IEnumerable<IEpsgTable>
    {
        OrderedDictionary Items;

        public EpsgTableCollection()
        {
            Items = new OrderedDictionary(20, StringComparer.OrdinalIgnoreCase);
        }

        public void Add(IEpsgTable table)
        {
            Items.Add(table.GetName(), table);
        }

        public IEpsgTable this[int index]
        {
            get { return (IEpsgTable)Items[index]; }
        }

        public IEpsgTable this[string name]
        {
            get { return (IEpsgTable)Items[name]; }
        }

        internal T Find<T>() where T : IEpsgTable
        {
            foreach (IEpsgTable table in this)
            {
                if (table is T)
                {
                    return (T)table;
                }
            }

            throw new KeyNotFoundException();
        }

        public void Clear()
        {
            Items.Clear();
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public IEnumerator<IEpsgTable> GetEnumerator()
        {
            foreach (IEpsgTable item in Items.Values)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
