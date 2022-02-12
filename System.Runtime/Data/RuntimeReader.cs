using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace System.Runtime.Data
{
    internal class RuntimeReader : DbDataReader
    {
        public RuntimeReader(int affectedRecords)
        {
            AffectedRecords = affectedRecords;
        }

        public RuntimeReader(IEnumerable<IDataRecord> e)
        {
            AffectedRecords = -1;
            Enumerator = e.GetEnumerator();
            Flag = Enumerator.MoveNext();
        }

        protected bool Flag;
        protected bool Closed;
        protected readonly int AffectedRecords;
        protected readonly IEnumerator<IDataRecord> Enumerator;

        public override void Close()
        {
            Enumerator.DisposeSafely();
        }

        public override int Depth
        {
            get { return 0; }
        }

        public override int FieldCount
        {
            get { return Closed ? 0 : Enumerator.Current.FieldCount; }
        }

        public override bool GetBoolean(int ordinal)
        {
            return Enumerator.Current.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return Enumerator.Current.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return Enumerator.Current.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return Enumerator.Current.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return Enumerator.Current.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return Enumerator.Current.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return Enumerator.Current.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return Enumerator.Current.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return Enumerator.Current.GetDouble(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            Enumerator.Reset();
            return Enumerator;
        }

        public override Type GetFieldType(int ordinal)
        {
            return Enumerator.Current.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return Enumerator.Current.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return Enumerator.Current.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return Enumerator.Current.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return Enumerator.Current.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return Enumerator.Current.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return Enumerator.Current.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return Enumerator.Current.GetOrdinal(name);
        }

        public override DataTable GetSchemaTable()
        {
            return default(DataTable);
        }

        public override string GetString(int ordinal)
        {
            return Enumerator.Current.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return Enumerator.Current.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return Enumerator.Current.GetValues(values);
        }

        public override bool HasRows
        {
            get { return false; }
        }

        public override bool IsClosed
        {
            get { return Closed; }
        }

        public override bool IsDBNull(int ordinal)
        {
            return Enumerator.Current.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            if (Flag)
            {
                Flag = false;
                return true;
            }

            if (Enumerator.MoveNext())
            {
                return true;
            }

            Closed = true;
            return false;
        }

        public override int RecordsAffected
        {
            get { return AffectedRecords; }
        }

        public override object this[string name]
        {
            get { return Enumerator.Current[name]; }
        }

        public override object this[int ordinal]
        {
            get { return Enumerator.Current[ordinal]; }
        }
    }
}
