﻿//////////////////////////////////////////////////////////////////////////////////////////////////
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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace System.Runtime
{
    public static class MethodCache
    {
        static MethodCache()
        {
            Properties = new DictionaryCache<int, PropertyInfo>();
            PropertyGets = new DictionaryCache<int, MethodInfo>();
            Methods = new DictionaryCache<int, MethodInfo>();
            GenericTypes = new DictionaryCache<int, Type>();
            GenericMethods = new DictionaryCache<int, MethodInfo>();
        }

        public const ParameterModifier[] EmptyModifiers = default(ParameterModifier[]);
        public const BindingFlags MemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.ExactBinding;

        public static readonly DictionaryCache<int, PropertyInfo> Properties;
        public static readonly DictionaryCache<int, MethodInfo> PropertyGets;
        public static readonly DictionaryCache<int, MethodInfo> Methods;
        public static readonly DictionaryCache<int, Type> GenericTypes;
        public static readonly DictionaryCache<int, MethodInfo> GenericMethods;

        public static readonly Type DataRecord = typeof(DataRecord);
        public static readonly Type IDataRecordSet = typeof(IDataRecordSet);
        public static readonly Type IRelationalDataRecord = typeof(IRelationalDataRecord);
        public static readonly Type IEnumerableRelationalDataRecord = typeof(IEnumerable<IRelationalDataRecord>);

        public static class String
        {
            static String()
            {
                IsEquals = Types.String.GetMethod("Equals", Types.String, Types.String);
                Concat = Types.String.GetMethod("Concat", Types.String, Types.String);
                CompareOrdinal = Types.String.GetMethod("CompareOrdinal", Types.String, Types.String);
                GetEnumerator = Types.String.GetMethod("GetEnumerator");

                Like = typeof(StringExtensions).GetMethod("Like", Types.String, Types.String);
                iLike = typeof(StringExtensions).GetMethod("iLike", Types.String, Types.String);
            }

            public static readonly MethodInfo IsEquals;
            public static readonly MethodInfo Concat;
            public static readonly MethodInfo CompareOrdinal;
            public static readonly MethodInfo GetEnumerator;

            public static readonly MethodInfo Like;
            public static readonly MethodInfo iLike;
        }

        public static class Decimal
        {
            static Decimal()
            {
                ConstInt32 = Types.Decimal.GetConstructor(Types.Int32);
                ConstInt64 = Types.Decimal.GetConstructor(Types.Int64);
                ConstSingle = Types.Decimal.GetConstructor(Types.Single);
                Add = Types.Decimal.GetMethod("Add", Types.Decimal, Types.Decimal);
                Divide = Types.Decimal.GetMethod("Divide", Types.Decimal, Types.Decimal);
                Multiply = Types.Decimal.GetMethod("Multiply", Types.Decimal, Types.Decimal);
                Compare = Types.Decimal.GetMethod("Compare", Types.Decimal, Types.Decimal);
                IsEquals = Types.Decimal.GetMethod("Equals", Types.Decimal, Types.Decimal);
            }

            public static readonly ConstructorInfo ConstInt32;
            public static readonly ConstructorInfo ConstInt64;
            public static readonly ConstructorInfo ConstSingle;

            public static readonly MethodInfo Add;
            public static readonly MethodInfo Divide;
            public static readonly MethodInfo Multiply;
            public static readonly MethodInfo Compare;
            public static readonly MethodInfo IsEquals;
        }

        public static class DateTime
        {
            static DateTime()
            {
                IsEquals = Types.DateTime.GetMethod("Equals", Types.DateTime, Types.DateTime);
                Compare = Types.DateTime.GetMethod("Compare", Types.DateTime, Types.DateTime);
            }

            public static readonly MethodInfo IsEquals;
            public static readonly MethodInfo Compare;
        }

        public static class Data
        {
            static Data()
            {
                DBNull = typeof(Convert).GetField("DBNull");
                IsDBNull = Types.IDataRecord.GetMethod("IsDBNull");
                DataRecordIndexer = Types.IDataRecord.GetIndexer(Types.Int32);
                DataRecordNamedIndexer = Types.IDataRecord.GetIndexer(Types.String);
                GetValue = Types.IDataRecord.GetMethod("GetValue", Types.Int32);
                GetOrdinal = Types.IDataRecord.GetMethod("GetOrdinal");
                CountOrDefault = DataRecord.GetMethod("CountOrDefault");
                LongCountOrDefault = DataRecord.GetMethod("LongCountOrDefault");
                AvgOrDefault = DataRecord.GetMethod("AvgOrDefault");
                MaxOrDefault = DataRecord.GetMethod("MaxOrDefault");
                MinOrDefault = DataRecord.GetMethod("MinOrDefault");
                SumOrDefault = DataRecord.GetMethod("SumOrDefault");
                ExecuteScalar = IDataRecordSet.GetMethod("ExecuteScalar");
                AsEnumerable = IDataRecordSet.GetMethod("AsEnumerable");
            }

            public static readonly FieldInfo DBNull;
            public static readonly MethodInfo IsDBNull;
            public static readonly MethodInfo DataRecordIndexer;
            public static readonly MethodInfo DataRecordNamedIndexer;
            public static readonly MethodInfo GetValue;
            public static readonly MethodInfo GetOrdinal;
            public static readonly MethodInfo CountOrDefault;
            public static readonly MethodInfo LongCountOrDefault;
            public static readonly MethodInfo AvgOrDefault;
            public static readonly MethodInfo MaxOrDefault;
            public static readonly MethodInfo MinOrDefault;
            public static readonly MethodInfo SumOrDefault;
            public static readonly MethodInfo ExecuteScalar;
            public static readonly MethodInfo AsEnumerable;
        }

        public static class DataConnection
        {
            static DataConnection()
            {
                OpenConnection = Types.IDbConnection.GetMethod("Open");
                CloseConnection = Types.IDbConnection.GetMethod("Close");
                ChangeDatabase = Types.IDbConnection.GetMethod("ChangeDatabase");
                SetConnectionString = Types.IDbConnection.GetProperty("ConnectionString").GetSetMethod();
            }

            public static readonly MethodInfo OpenConnection;
            public static readonly MethodInfo CloseConnection;
            public static readonly MethodInfo ChangeDatabase;
            public static readonly MethodInfo SetConnectionString;
        }

        public static class DataCommand
        {
            static DataCommand()
            {
                Connection = Types.IDbCommand.GetProperty("Connection").GetGetMethod();
                Parameters = Types.IDbCommand.GetProperty("Parameters").GetGetMethod();
                ParametersIndexer = Types.IDataParameterCollection.GetIndexer(Types.String);
                GetParameterValue = Types.IDataParameter.GetProperty("Value").GetGetMethod();
            }

            public static readonly MethodInfo Connection;
            public static readonly MethodInfo Parameters;
            public static readonly MethodInfo ParametersIndexer;
            public static readonly MethodInfo GetParameterValue;
        }

        public static class List
        {
            static List()
            {
                Indexer = Types.IList.GetIndexer(Types.Int32);
            }

            public static readonly MethodInfo Indexer;
        }

        public static class Linq
        {
            static Linq()
            {
                AsEnumerable = Types.Enumerable.GetGenericMethod("AsEnumerable", Types.IEnumerableT);
                Cast = Types.Enumerable.GetMethod("Cast", Types.IEnumerable);
                First = Types.Enumerable.GetGenericMethod("First", Types.IEnumerableT);
                FirstOrDefault = Types.Enumerable.GetGenericMethod("FirstOrDefault", Types.IEnumerableT);
                Select = Types.Enumerable.GetGenericMethod("Select", Types.IEnumerableT, Types.FuncT);
                OrderBy = Types.Enumerable.GetGenericMethod("OrderBy", Types.IEnumerableT, Types.FuncT, Types.IComparerT);
                OrderByDescending = Types.Enumerable.GetGenericMethod("OrderByDescending", Types.IEnumerableT, Types.FuncT, Types.IComparerT);
                ThenBy = Types.Enumerable.GetGenericMethod("ThenBy", Types.IEnumerableT, Types.FuncT, Types.IComparerT);
                ThenByDescending = Types.Enumerable.GetGenericMethod("ThenByDescending", Types.IEnumerableT, Types.FuncT, Types.IComparerT);
                ToList = Types.Enumerable.GetGenericMethod("ToList", Types.IEnumerableT);
                Contains = Types.Enumerable.GetGenericMethod("Contains", Types.IEnumerableT, Types.GenericArgument);
                Range = Types.Enumerable.GetMethod("Range", Types.Int32, Types.Int32);
            }

            public static readonly MethodInfo AsEnumerable;
            public static readonly MethodInfo Cast;
            public static readonly MethodInfo First;
            public static readonly MethodInfo FirstOrDefault;
            public static readonly MethodInfo Select;
            public static readonly MethodInfo OrderBy;
            public static readonly MethodInfo ThenBy;
            public static readonly MethodInfo OrderByDescending;
            public static readonly MethodInfo ThenByDescending;
            public static readonly MethodInfo ToList;
            public static readonly MethodInfo Contains;
            public static readonly MethodInfo Range;
        }

        public static class Dictionary
        {
            static Dictionary()
            {
                ValueByKey = Types.IDictionary.GetIndexer(Types.Object);
            }

            public static readonly MethodInfo ValueByKey;
        }

        public static class SupportProperty
        {
            static SupportProperty()
            {
                GetPropertyValue = typeof(ISupportProperty).GetMethod("GetPropertyValue");
            }

            public static readonly MethodInfo GetPropertyValue;
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
            PropertyInfo value;
            TryGetProperty(type, name, out value);
            return value;
        }

        public static bool TryGetProperty(Type type, string name, out PropertyInfo value)
        {
            int key = GetKey(type, name);

            if (Properties.TryGetValue(key, out value))
            {
                return true;
            }

            value = type.GetProperty(name, MemberFlags);
            value = value ?? type.GetProperty(name, MemberFlags | ~BindingFlags.ExactBinding);
            value = value ?? type.GetInterfaces().Select(e => e.GetProperty(name, MemberFlags)).FirstOrDefault();

            if (value.IsNull())
            {
                return false;
            }

            Properties.Add(key, value);
            return true;
        }

        public static MethodInfo GetMethod(Type type, string name)
        {
            MethodInfo value;
            TryGetMethod(type, name, out value);
            return value;
        }

        public static MethodInfo GetMethod(Type type, string name, params Type[] types)
        {
            MethodInfo value;
            TryGetMethod(type, name, out value, types);
            return value;
        }

        public static bool TryGetMethod(Type type, string name, out MethodInfo value)
        {
            int key = GetKey(type, name);

            if (PropertyGets.TryGetValue(key, out value))
            {
                return true;
            }

            value = type.GetMethod(name, MemberFlags, Type.DefaultBinder, Types.TypeArrayEmpty, EmptyModifiers);

            if (value.IsNull())
            {
                return false;
            }

            PropertyGets.Add(key, value);
            return true;
        }

        public static bool TryGetMethod(Type type, string name, out MethodInfo value, Type[] types)
        {
            int key = GetKey(type.Name, name, types);

            if (Methods.TryGetValue(key, out value))
            {
                return true;
            }

            value = type.GetMethod(name, MemberFlags, Type.DefaultBinder, types, EmptyModifiers);
            value = value ?? type.GetMethod(name, MemberFlags & ~BindingFlags.ExactBinding, Type.DefaultBinder, types, EmptyModifiers);
            value = value ?? type.GetInterfaces().Select(e => e.GetMethod(name, MemberFlags, Type.DefaultBinder, types, EmptyModifiers)).FirstOrDefault();

            if (value.IsNull())
            {
                return false;
            }

            Methods.Add(key, value);
            return true;
        }

        public static Type MakeGenericType(this Type type, params Type[] typeArguments)
        {
            Type value;
            int key = GetKey(type.Name, typeArguments);

            if (GenericTypes.TryGetValue(key, out value))
            {
                return value;
            }

            GenericTypes.Add(key, value = type.MakeGenericType(typeArguments));
            return value;
        }

        public static MethodInfo MakeGenericMethod(this MethodInfo method, params Type[] typeArguments)
        {
            MethodInfo value;
            int key = GetKey(method.Name, typeArguments);

            if (GenericMethods.TryGetValue(key, out value))
            {
                return value;
            }

            GenericMethods.Add(key, value = method.MakeGenericMethod(typeArguments));
            return value;
        }

        static int GetKey(Type type, string name)
        {
            int n = Hash.Seed;

            unchecked
            {
                n = n * Hash.Step + type.GetHashCode();
                n = n * Hash.Step + name.GetHashCode();
            }

            return n;
        }

        static int GetKey(string name, Type[] typeArguments)
        {
            return GetKey(string.Empty, name, typeArguments);
        }

        static int GetKey(string typeName, string name, Type[] typeArguments)
        {
            int n = Hash.Seed;

            unchecked
            {
                n = n * Hash.Step + typeName.GetHashCode();
                n = n * Hash.Step + name.GetHashCode();

                foreach (Type arg in typeArguments)
                {
                    n = n * Hash.Step + arg.GetHashCode();
                }
            }

            return n;
        }
    }
}
