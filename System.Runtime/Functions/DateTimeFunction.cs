﻿//////////////////////////////////////////////////////////////////////////////////////////////////
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
    internal class DateTimeFunction : ExpressionFunction, IExpressionFunctionCollection
    {
        public DateTimeFunction()
        {
        }

        public DateTimeFunction(string name)
        {
            Function = (DateTimeFunctions)Enum.Parse(typeof(DateTimeFunctions), name);
        }

        protected DateTimeFunctions Function;

        public override bool IsEmpty()
        {
            return ValueOperand.IsNull() || ValueOperand.IsEmpty();
        }

        public override bool HasLeftOperand()
        {
            return false;
        }

        public override string GetFunctionName()
        {
            return Enum.GetName(typeof(DateTimeFunctions), Function);
        }

        public IEnumerable<string> GetFunctionNames()
        {
            foreach (string name in Enum.GetNames(typeof(DateTimeFunctions)))
            {
                yield return name;
            }
        }

        public IExpressionOperatorCollection Parameters
        {
            get;
            protected set;
        }

        public override bool TryParse(ITokenEnumerator e, IExpressionOperator leftOperand, out IExpressionFunction result)
        {
            if (e.Current.Equals('(') && e.NextIs(')'))
            {
                IValueOperand operand;

                e.MoveNext();
                e.MoveNext();

                switch (Function)
                {
                    case DateTimeFunctions.GetDate:
                        operand = new ValueOperand(Types.DateTime, "Now");
                        break;
                    default:
                        throw new NotImplementedException();
                }

                result = new DateTimeFunction { ValueOperand = operand };
                return true;
            }

            result = default(IExpressionFunction);
            return false;
        }

        protected override Type OnPutInstructions(IInstructionEventArgs e)
        {
            return ValueOperand.PutInstructions(e);
        }

        protected override string OnLegacyToString(ILegacyToStringVisitor visitor)
        {
            return ValueOperand.LegacyToString(visitor).Replace("Now", Enum.GetName(typeof(DateTimeFunctions), Function));
        }

        public override IEnumerator<IExpressionOperator> GetEnumerator()
        {
            yield return ValueOperand;
        }
    }
}
