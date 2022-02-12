using System.Runtime;

namespace System.ComponentModel
{
    public interface ISupportCriteria
    {
        bool AddCriteria(IExpressionOperator operand);
    }
}
