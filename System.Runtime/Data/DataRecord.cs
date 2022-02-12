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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;

namespace System.Data
{
    public class DataRecord : IDataRecord, ISupportProperty, ISupportInstructions
    {
        static DataRecord()
        {
            EmptyRows = new IDataRecord[] { };
            Aggregates = new Dictionary<int, IExpressionOperator>();
            FieldGetRow = typeof(DataRecord).GetField("Row", BindingFlags.Public | BindingFlags.Instance);
        }

        public static readonly FieldInfo FieldGetRow;
        protected static readonly IDataRecord[] EmptyRows;

        public DataRecord()
        {
        }

        public DataRecord(string tableName)
        {
            ComponentName = tableName;
        }

        public DataRecord(IEnumerable<IDataRecord> childRows)
        {
            ChildRows = childRows;
        }

        public static readonly Dictionary<int, IExpressionOperator> Aggregates;

        protected bool HasMembers;
        protected MemberAccessorCollection Members;
        protected readonly IEnumerable<IDataRecord> ChildRows = EmptyRows;

        public bool HasRow;
        public IDataRecord Row;

        [Browsable(false)]
        public string ComponentName
        {
            get;
            protected set;
        }

        [Browsable(false)]
        public virtual int FieldCount
        {
            get
            {
                if (HasRow)
                {
                    return Row.FieldCount;
                }

                return Properties.Count;
            }
        }

        [Browsable(false)]
        public MemberAccessorCollection Properties
        {
            get
            {
                if (HasMembers)
                {
                    return Members;
                }

                HasMembers = true;
                Members = GetProperties();

                return Members;
            }
        }

        public bool GetBoolean(int i)
        {
            return (bool)this[i];
        }

        public byte GetByte(int i)
        {
            return (byte)this[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            return (char)this[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            return default(IDataReader);
        }

        public string GetDataTypeName(int i)
        {
            if (HasRow)
            {
                return Row.GetDataTypeName(i);
            }

            return GetFieldType(i).Name;
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)this[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)this[i];
        }

        public double GetDouble(int i)
        {
            return (double)this[i];
        }

        public Type GetFieldType(int i)
        {
            return GetPropertyType(GetName(i));
        }

        public float GetFloat(int i)
        {
            return (float)this[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)this[i];
        }

        public short GetInt16(int i)
        {
            return (short)this[i];
        }

        public int GetInt32(int i)
        {
            return (int)this[i];
        }

        public long GetInt64(int i)
        {
            return (long)this[i];
        }

        public virtual string GetName(int i)
        {
            if (HasRow)
            {
                return Row.GetName(i);
            }

            return Properties[i].Name;
        }

        public virtual int GetOrdinal(string name)
        {
            if (HasRow)
            {
                return Row.GetOrdinal(name);
            }

            return Properties.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return (string)this[i];
        }

        public object GetValue(int i)
        {
            return this[i];
        }

        public int GetValues(object[] values)
        {
            if (HasRow)
            {
                return Row.GetValues(values);
            }

            int n = FieldCount;

            for (int i = 0; i < n; i++)
            {
                values[i] = this[i];
            }

            return n;
        }

        public bool IsDBNull(int i)
        {
            if (HasRow)
            {
                return Row.IsDBNull(i);
            }

            return this[i] == Convert.DBNull;
        }

        public virtual object this[string name]
        {
            get
            {
                if (HasRow)
                {
                    return Row[name];
                }

                return Properties[name].GetValue(this);
            }
        }

        public virtual object this[int i]
        {
            get
            {
                if (HasRow)
                {
                    return Row[i];
                }

                return Properties[i].GetValue(this);
            }
        }

        public virtual int PutMemberInstructions(IInstructionEventArgs e, string name, IEnumerator<string> members, out Type returnType)
        {
            int n = Row.GetOrdinal(members.Current);

            if (n >= 0)
            {
                e.Generator.Emit(OpCodes.Ldarg_0);
                OnPutMemberInstructions(e, n, returnType = Row.GetFieldType(n));

                return 1;
            }

            returnType = default(Type);
            return 0;
        }

        public virtual Type GetPropertyType(string name)
        {
            if (HasRow)
            {
                int i;

                if ((i = Row.GetOrdinal(name)) >= 0)
                {
                    return Row.GetFieldType(i);
                }
            }

            return Properties[name].PropertyType;
        }

        public virtual object GetPropertyValue(string name)
        {
            if (HasRow)
            {
                return Row[name];
            }

            return Properties[name].GetValue(this);
        }

        public int CountOrDefault()
        {
            return GetChildRows().Count();
        }

        public object AvgOrDefault(int key)
        {
            IExpressionOperator operand;

            if (Aggregates.TryGetValue(key, out operand))
            {
                switch (Type.GetTypeCode(operand.ReturnType))
                {
                    case TypeCode.Decimal:
                        return GetChildRows().Select(o => (decimal)operand.Evaluate(o)).DefaultIfEmpty(default(decimal)).Average();
                }

                return operand.ReturnType.Default();
            }

            return default(object);
        }

        public object MaxOrDefault(int key)
        {
            IExpressionOperator operand;

            if (Aggregates.TryGetValue(key, out operand))
            {
                return GetChildRows().Select(o => operand.Evaluate(o)).DefaultIfEmpty(operand.ReturnType.Default()).Max();
            }

            return default(object);
        }

        public object MinOrDefault(int key)
        {
            IExpressionOperator operand;

            if (Aggregates.TryGetValue(key, out operand))
            {
                return GetChildRows().Select(o => operand.Evaluate(o)).DefaultIfEmpty(operand.ReturnType.Default()).Min();
            }

            return default(object);
        }

        public object SumOrDefault(int key)
        {
            IExpressionOperator operand;

            if (Aggregates.TryGetValue(key, out operand))
            {
                switch (Type.GetTypeCode(operand.ReturnType))
                {
                    case TypeCode.Decimal:
                        return GetChildRows().Select(o => (decimal)operand.Evaluate(o)).DefaultIfEmpty(default(decimal)).Sum();
                }
            }

            return default(object);
        }

        public IEnumerable<IDataRecord> GetChildRows()
        {
            return ChildRows;
        }

        protected virtual void OnPutMemberInstructions(IInstructionEventArgs e, int n, Type returnType)
        {
            e.Generator.Emit(OpCodes.Ldfld, FieldGetRow);
            e.Generator.Emit(OpCodes.Ldc_I4, n);
            e.Generator.Emit(OpCodes.Callvirt, MethodCache.Data.DataRecordIndexer);
            e.Generator.Emit(OpCodes.Unbox_Any, returnType);
        }

        protected virtual MemberAccessorCollection GetProperties()
        {
            return MemberAccessor.GetMemberAccessors(GetType());
        }

        public static IRelationalDataRecord Concat(IDataRecord row, string alias, IDataRecord other, string otherAlias)
        {
            var a = row as IRelationalDataRecord ?? new RelationalDataRecord(new DataSource(default(IDataSet), alias));
            var b = other as IRelationalDataRecord ?? new RelationalDataRecord(new DataSource(default(IDataSet), otherAlias));

            a.Row = row;
            b.Row = other;

            if (a.IsJoined)
            {
                a.Rows[0] = b;
                return a;
            }

            a.IsJoined = true;
            a.Rows = new IRelationalDataRecord[] { b };

            return a;
        }
    }
}
