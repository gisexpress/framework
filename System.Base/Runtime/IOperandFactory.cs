namespace System.Runtime
{
    public interface IOperandFactory
    {
        IExpressionOperator Parse(string expression, object component);
    }
}
