namespace System.Runtime
{
    public interface IIsOperator : IExpressionFunction
    {
        bool IsNot
        {
            get;
        }

        IExpressionOperator LeftOperand
        {
            get;
        }
    }
}
