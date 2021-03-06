//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright © GISExpress 2015-2017. All Rights Reserved.
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
using System.IO;

namespace System.Runtime
{
    internal class InsertOperator : ExpressionFunction, IInsertOperator
    {
        public override bool IsEmpty()
        {
            return (Table.IsNull() || Table.IsEmpty()) || (Fields.IsNull() || Fields.IsEmpty()) || (Values.IsNull() || Values.IsEmpty());
        }

        public override string GetFunctionName()
        {
            return Constants.Sql.KeywordInsert;
        }

        public override bool HasLeftOperand()
        {
            return false;
        }

        public override string Name
        {
            get { return Table.Name; }
            set { }
        }

        public IValueOperand Table
        {
            get;
            set;
        }

        public IExpressionOperatorCollection Fields
        {
            get;
            set;
        }

        public IExpressionOperatorCollection Values
        {
            get;
            set;
        }

        public IExpressionOperator Operand
        {
            get;
            set;
        }

        public override bool TryParse(ITokenEnumerator e, IExpressionOperator leftOperand, out IExpressionFunction result)
        {
            var o = new InsertOperator();

            if (e.Current.Equals(Constants.Sql.KeywordInto))
            {
                e.MoveNext();
            }

            result = o;
            o.Fields = new ExpressionOperatorCollection();
            o.Table = (IValueOperand)Read(e, o.Fields);

            if (e.Current.Equals('('))
            {
                if (ReadParameters(e, o.Fields))
                {
                    if (e.Current.Equals(Constants.Sql.KeywordValues) && e.MoveNext() && e.Current.Equals('('))
                    {
                        o.Values = new ExpressionOperatorCollection();
                        ReadParameters(e, o.Values);
                        return true;
                    }
                }
            }
            else
            {
                o.Operand = Read(e, o.Fields);
                return true;
            }

            throw e.SyntaxError();
        }

        protected override Type OnPutInstructions(IInstructionEventArgs e)
        {
            //Fields.OfType<IOperandProperty>().ToList().ForEach(o => o.Quote = Quotes.Single);

            //e.Generator.Emit(OpCodes.Ldarg_0);
            //e.LoadValue(Table.Name);
            //e.LoadArray(Fields, Types.String);
            //e.LoadArray(Values);
            //e.Generator.Emit(OpCodes.Call, e.ComponentType.GetMethod("Insert"));
            //e.Generator.Emit(OpCodes.Ldc_I4, 1);

            //return Types.Int32;

            throw new NotSupportedException();
        }

        public override int ExecuteNonQuery(object component)
        {
            throw new NotImplementedException();
        }

        protected override string OnLegacyToString(ILegacyToStringVisitor visitor)
        {
            return GetLegacyStrings(visitor).Join(" ");
        }

        protected IEnumerable<string> GetLegacyStrings(ILegacyToStringVisitor visitor)
        {
            yield return Constants.Sql.KeywordInsert;
            yield return Constants.Sql.KeywordInto;
            yield return Table.LegacyToString(visitor);

            if (Operand.IsNull())
            {
                yield return string.Concat("(", Fields.LegacyToString(visitor), ")");
                yield return Constants.Sql.KeywordValues;
                yield return string.Concat("(", Values.LegacyToString(visitor), ")");
            }
            else
            {
                yield return Operand.LegacyToString(visitor);
            }
        }

        public override IEnumerator<IExpressionOperator> GetEnumerator()
        {
            yield return Table;

            foreach (IExpressionOperator o in Fields)
            {
                yield return o;
            }

            foreach (IExpressionOperator o in Values)
            {
                yield return o;
            }
        }
    }
}
