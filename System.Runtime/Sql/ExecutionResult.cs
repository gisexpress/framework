using System.Collections.Generic;
using System.Data;

namespace System.Runtime
{
    public class ExecutionResult
    {
        public ExecutionResultKind Type;

        public object Value;
        public int RecordsAffected;
        public IEnumerable<IDataRecord> RowSet;
    }

    public enum ExecutionResultKind
    {
        NonQuery,
        Scalar,
        Reader
    }
}
