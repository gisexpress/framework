//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright © GISExpress 2015-2017. All Rights Reserved.
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
using System.Reflection;

namespace System.Runtime
{
    public class MemberAccessor : IMemberAccessor
    {
        protected static readonly Dictionary<Type, MemberAccessorCollection> Cache;

        static MemberAccessor()
        {
            Cache = new Dictionary<Type, MemberAccessorCollection>();
        }

        protected MemberAccessor()
        {
        }

        public MemberAccessor(string name)
        {
            MemberName = name;
        }

        public MemberAccessor(string name, Type type)
        {
            MemberName = name;
            MemberReturnType = type;
        }

        public MemberAccessor(PropertyInfo property)
        {
            MemberName = property.Name;
            MemberReturnType = property.PropertyType;
            MethodGet = property.GetGetMethod().CreateDelegate();
        }

        protected int MemberOrdinal;
        protected Type MemberReturnType;
        protected readonly string MemberName;
        protected readonly Func<object, object> MethodGet;

        public string Name
        {
            get { return MemberName; }
        }

        public int Ordinal
        {
            get { return MemberOrdinal; }
            set { MemberOrdinal = value; }
        }

        public virtual Type PropertyType
        {
            get { return MemberReturnType; }
            set { MemberReturnType = value; }
        }

        public virtual object GetValue(object component)
        {
            return MethodGet(component);
        }

        public static MemberAccessorCollection GetMemberAccessors(Type componentType)
        {
            MemberAccessorCollection value;

            if (!Cache.TryGetValue(componentType, out value))
            {
                value = new MemberAccessorCollection();

                foreach (PropertyInfo property in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    if (property.GetIndexParameters().Length == 0 && property.IsBrowsable())
                    {
                        value.Add(property);
                    }
                }

                Cache.Add(componentType, value);
            }

            return value;
        }
    }
}
