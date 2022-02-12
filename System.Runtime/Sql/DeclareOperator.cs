using System.Collections.Generic;
using System.Data;
using System.IO;

namespace System.Runtime
{
    class DeclareOperator : ExpressionFunction, IDeclareOperator
    {
        public string ParameterName
        {
            get;
            protected set;
        }

        public string ParameterType
        {
            get;
            protected set;
        }

        public string Expression
        {
            get;
            protected set;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(ParameterName) || string.IsNullOrEmpty(Expression);
        }

        public override string GetFunctionName()
        {
            return Constants.Sql.KeywordDeclare;
        }

        public override bool HasLeftOperand()
        {
            return false;
        }

        public override bool TryParse(ITokenEnumerator e, IExpressionOperator leftOperand, out IExpressionFunction result)
        {
            var o = new DeclareOperator { ParameterName = e.Current.StringValue };

            if (e.MoveNext() && e.Current.Equals('=') && e.MoveNext())
            {
                string parameter = e.Current.StringValue;

                if (e.MoveNext() && e.Current.Equals(':'))
                {
                    if (e.MoveNext() && e.Current.Equals(':'))
                    {
                        e.MoveNext();
                    }

                    o.ParameterType = parameter;
                    o.Expression = string.Concat(parameter, ".", e.ReadToEnd(';'));
                }
                else
                {
                    o.Expression = e.ReadToEnd(';');
                }

                result = o;
                return true;
            }

            throw e.SyntaxError();
        }

        protected override Type OnPutInstructions(IInstructionEventArgs e)
        {
            var command = (IDbCommand)e.Component;
            IDbDataParameter parameter = command.CreateParameter();

            parameter.ParameterName = ParameterName;
            parameter.Value = Evaluator.Eval(Expression, MethodCache.GetProperty(e.ComponentType, ParameterType).GetValue(e.Component, null));
            command.Parameters.Add(parameter);
            
            e.LoadValue(1);
            return Types.Int32;
        }

        public override IEnumerator<IExpressionOperator> GetEnumerator()
        {
            yield break;
        }
    }
}
