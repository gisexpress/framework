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
    public partial class UnaryOperator : ExpressionOperator, IValueOperand
    {
        public UnaryOperator()
        {
        }

        public UnaryOperator(UnaryOperatorType operatorType, IExpressionOperator operand)
        {
            OperatorType = operatorType;
            Operand = operand;
        }

        public override bool IsEmpty()
        {
            return OperatorType == UnaryOperatorType.None || Operand == null;
        }

        public override bool IsLogical()
        {
            return true;
        }

        public override bool IsHandled()
        {
            return Operand.IsHandled();
        }

        public override void Clear()
        {
            OperatorType = UnaryOperatorType.None;
            Operand.Clear();
        }

        public UnaryOperatorType OperatorType
        {
            get;
            set;
        }

        public IExpressionOperator Operand
        {
            get;
            set;
        }

        protected override Type OnPutInstructions(IInstructionEventArgs e)
        {
            Type r = Operand.PutInstructions(e);

            switch (OperatorType)
            {
                case UnaryOperatorType.Minus: e.Negates(); break;
                case UnaryOperatorType.Not: e.Not(); break;
                case UnaryOperatorType.BitwiseNot: e.BitwiseNot(); break;
            }

            return r;
        }

        internal static bool TryParseOperator(ITokenEnumerator e, out UnaryOperatorType operatorType)
        {
            var current = e.Current as CharToken;
            operatorType = UnaryOperatorType.None;

            if (e.Current.Equals('+'))
            {
                operatorType = UnaryOperatorType.Plus;
            }
            else if (e.Current.Equals('-'))
            {
                operatorType = UnaryOperatorType.Minus;
            }
            else if (e.Current.Equals('~'))
            {
                operatorType = UnaryOperatorType.BitwiseNot;
            }
            else if (e.Current.Equals("Not"))
            {
                operatorType = UnaryOperatorType.Not;
            }

            if (operatorType != UnaryOperatorType.None)
            {
                if (e.MoveNext() && e.Current is CharToken && !e.Current.Equals('('))
                {
                    throw new InvalidOperatorException(string.Concat(current, e.Current));
                }

                return true;
            }

            return false;
        }

        protected string ToSymbol(UnaryOperatorType operatorType)
        {
            switch (OperatorType)
            {
                case UnaryOperatorType.Plus: return "+";
                case UnaryOperatorType.Minus: return "-";
                case UnaryOperatorType.BitwiseNot: return "~";
                case UnaryOperatorType.Not: return "Not";
            }

            throw new NotImplementedException();
        }

        protected override string OnLegacyToString(ILegacyToStringVisitor visitor)
        {
            if (Operand.IsGroup)
            {
                return string.Concat(ToSymbol(OperatorType), '(', Operand.LegacyToString(visitor), ')');
            }

            return string.Concat(ToSymbol(OperatorType), ' ', Operand.LegacyToString(visitor));
        }

        #region IValueOperand

        bool IValueOperand.SupportsArguments
        {
            get { return Operand is IValueOperand; }
        }

        Type IValueOperand.ReturnType
        {
            get { return default(Type); }
        }

        Quotes IValueOperand.Quote
        {
            get { return (Operand as IValueOperand).Quote; }
            set { (Operand as IValueOperand).Quote = value; }
        }

        void IValueOperand.Add(string name)
        {
            (Operand as IValueOperand).Add(name);
        }

        void IValueOperand.AddWithArgument(string name, IExpressionOperator arg)
        {
            (Operand as IValueOperand).AddWithArgument(name, arg);
        }

        void IValueOperand.AddWithArguments(string name, IExpressionOperatorCollection args)
        {
            (Operand as IValueOperand).AddWithArguments(name, args);
        }

        IList<IExpressionOperatorCollection> IValueOperand.Arguments
        {
            get { return (Operand as IValueOperand).Arguments; }
        }

        object IValueOperand.Value
        {
            get { return (Operand as IValueOperand).Value; }
            set { (Operand as IValueOperand).Value = value; }
        }

        string IValueOperand.ToString()
        {
            return (Operand as IValueOperand).ToString();
        }

        #endregion

        public override IEnumerator<IExpressionOperator> GetEnumerator()
        {
            yield return Operand;
        }
    }
}