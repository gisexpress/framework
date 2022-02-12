namespace System.Runtime
{
    public interface IDeclareOperator : IExpressionFunction
    {
        string ParameterName
        {
            get;
        }

        string ParameterType
        {
            get;
        }

        string Expression
        {
            get;
        }
    }
}
