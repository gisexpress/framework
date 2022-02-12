﻿////////////////////////////////////////////////////////////////////////////////////////////////////
////
////  Copyright © GISExpress 2015-2022. All Rights Reserved.
////  
////  GISExpress .NET API and Component Library
////  
////  The entire contents of this file is protected by local and International Copyright Laws.
////  Unauthorized reproduction, reverse-engineering, and distribution of all or any portion of
////  the code contained in this file is strictly prohibited and may result in severe civil and 
////  criminal penalties and will be prosecuted to the maximum extent possible under the law.
////  
////  RESTRICTIONS
////  
////  THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES ARE CONFIDENTIAL AND PROPRIETARY TRADE SECRETS OF GISExpress
////  THE REGISTERED DEVELOPER IS LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET COMPONENTS AS PART OF AN EXECUTABLE PROGRAM ONLY.
////  
////  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE
////  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT
////  AND PERMISSION FROM GISExpress
////  
////  CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.
////  
////  Warning: This content was generated by GISExpress tools.
////  Changes to this content may cause incorrect behavior and will be lost if the content is regenerated.
////
/////////////////////////////////////////////////////////////////////////////////////////////////////

//using System.Data;

//namespace System
//{
//    public static class DataExtensions
//    {
//        public static T Get<T>(this DataRow r, int index)
//        {
//            return Get<T>(r, null, index);
//        }

//        public static T Get<T>(this DataRow r, string name)
//        {
//            return Get<T>(r, name, -1);
//        }

//        public static T Get<T>(this DataRow r, int index, DataRowVersion version)
//        {
//            return Get<T>(r, null, index, version);
//        }

//        public static T Get<T>(this DataRow r, string name, DataRowVersion version)
//        {
//            return Get<T>(r, name, -1, version);
//        }

//        static T Get<T>(DataRow r, string name, int index)
//        {
//            return Get<T>(r, name, index, r.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current);
//        }

//        static T Get<T>(DataRow r, string name, int index, DataRowVersion version)
//        {
//            var c = index >= 0 ? r.Table.Columns[index] : r.Table.Columns[name];

//            if (c.HasValue() && !r.IsNull(c, version))
//            {
//                return r[c, version].ConvertTo<T>();
//            }

//            return default(T);
//        }

//        public static T Get<T>(this IDataRecord r, int i)
//        {
//            if (!r.IsDBNull(i))
//            {
//                return r.GetValue(i).ConvertTo<T>();
//            }

//            return default(T);
//        }

//        public static T Get<T>(this IDataRecord r, string name)
//        {
//            return Get<T>(r, r.GetOrdinal(name));
//        }
//    }
//}
