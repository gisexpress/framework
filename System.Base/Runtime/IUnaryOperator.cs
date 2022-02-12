namespace System.Runtime
{
    public interface IUnaryOperator : IExpressionOperator, IValueOperand
    {
        UnaryOperatorType OperatorType
        {
            get;
        }

        IExpressionOperator Operand
        {
            get;
        }
    }
}
