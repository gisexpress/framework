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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace System.CoordinateSystems
{
    internal class AxisInfoCollection : IAxisInfoCollection
    {
        public AxisInfoCollection()
        {
        }

        public AxisInfoCollection(IEnumerable<IAxisInfo> collection)
        {
            foreach (IAxisInfo i in collection)
            {
                Add(i);
            }
        }

        public static AxisInfoCollection Default
        {
            get { return new AxisInfoCollection(new[] { AxisInfo.X, AxisInfo.Y }); }
        }

        protected OrderedDictionary Items = new OrderedDictionary(3);

        public bool Add(IAxisInfo axis)
        {
            return Add(axis.Name, axis.Orientation);
        }

        public bool Add(string name, AxisOrientation orientation)
        {
            if (!Items.Contains(orientation))
            {
                Items.Add(orientation, new AxisInfo(name, orientation));
                return true;
            }

            return false;
        }

        public IAxisInfo this[int index]
        {
            get { return Items[index] as IAxisInfo; }
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public bool IsDefault
        {
            get { return IsEquivalent(Default); }
        }

        public bool IsEquivalent(params IAxisInfo[] args)
        {
            return IsEquivalent(new AxisInfoCollection(args));
        }

        public bool IsEquivalent(IAxisInfoCollection other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            for (int n = 0; n < Count; n++)
            {
                if (!Items.Contains(other[n].Orientation))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Read(ITokenEnumerator e, out AxisInfoCollection value)
        {
            AxisInfo axis;
            value = null;

            if (e.Current.Equals(','))
            {
                e.MoveNext();
            }

            while (AxisInfo.Read(e, out axis))
            {
                value = value ?? new AxisInfoCollection();
                value.Add(axis);

                if (e.Current.Equals(','))
                {
                    if (!e.MoveNext())
                    {
                        break;
                    }
                }
            }

            return value.HasValue();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public IAxisInfoCollection Clone()
        {
            var c = new AxisInfoCollection { Items = new OrderedDictionary() };

            foreach (AxisInfo i in Items.Values)
            {
                c.Items.Add(i.Orientation, i);
            }

            return c;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IAxisInfo> GetEnumerator()
        {
            foreach (AxisInfo i in Items.Values)
            {
                yield return i;
            }
        }
    }
}