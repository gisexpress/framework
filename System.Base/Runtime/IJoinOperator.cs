using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Runtime
{
    public enum JoinType
    {
        Join,
        Inner,
        Outer,
        Left,
        Right,
        Spatial
    }

    public interface IJoinOperator : IExpressionFunction
    {
        JoinType Type
        {
            get;
        }

        IExpressionOperator Outer
        {
            get;
        }

        IExpressionOperator Inner
        {
            get;
        }

        IExpressionOperator Operand
        {
            get;
        }
    }
}
