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
using System.Reflection.Emit;

namespace System.Runtime
{
    public partial class BinaryOperator : ExpressionOperator, IBinaryOperator, IValueOperand
    {
        public BinaryOperator()
        {
        }

        public BinaryOperator(BinaryOperatorType operatorType, IExpressionOperator leftOperand, object literal)
            : this(operatorType, leftOperand, new ValueOperand(literal))
        {
        }

        public BinaryOperator(BinaryOperatorType operatorType, IExpressionOperator leftOperand, IExpressionOperator rightOperand)
        {
            OperatorType = operatorType;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        protected LocalBuilder LeftLocal;
        protected LocalBuilder RightLocal;

        protected TypeCode LeftType;
        protected TypeCode RightType;

        public override string Name
        {
            get { return base.Name ?? LeftOperand.Name ?? RightOperand.Name; }
            set { base.Name = value; }
        }

        public override bool IsEmpty()
        {
            return OperatorType == BinaryOperatorType.None || LeftOperand.IsNull() || RightOperand.IsNull();
        }

        public override bool IsConstant()
        {
            return LeftOperand.IsConstant() || RightOperand.IsConstant();
        }

        public override bool IsLogical()
        {
            switch (OperatorType)
            {
                case BinaryOperatorType.And:
                case BinaryOperatorType.Or:
                case BinaryOperatorType.Equals:
                case BinaryOperatorType.NotEquals:
                case BinaryOperatorType.Greater:
                case BinaryOperatorType.GreaterOrEqual:
                case BinaryOperatorType.Less:
                case BinaryOperatorType.LessOrEqual:
                case BinaryOperatorType.Like:
                case BinaryOperatorType.iLike:
                case BinaryOperatorType.LikeAll:
                case BinaryOperatorType.iLikeAll:
                    return true;
            }

            return false;
        }

        public override bool IsHandled()
        {
            return Handled || (LeftOperand.IsHandled() && RightOperand.IsHandled());
        }

        public override void Clear()
        {
            OperatorType = BinaryOperatorType.None;
            if (LeftOperand.HasValue()) LeftOperand.Clear();
            if (LeftOperand.HasValue()) RightOperand.Clear();
        }

        public string OperatorSymbol
        {
            get { return ToSymbol(OperatorType); }
        }

        public BinaryOperatorType OperatorType
        {
            get;
            set;
        }

        public IExpressionOperator LeftOperand
        {
            get;
            set;
        }

        public IExpressionOperator RightOperand
        {
            get;
            set;
        }

        protected override Type OnPutInstructions(IInstructionEventArgs e)
        {
            try
            {
                switch (OperatorType)
                {
                    case BinaryOperatorType.And:
                        return PutInstructionsAnd(e);

                    case BinaryOperatorType.Or:
                        return PutInstructionsOr(e);
                }

                return PutBinaryInstructions(e);
            }
            catch (MissingMemberException innerException)
            {
                throw new ExpressionParserException(string.Concat(LeftOperand.ReturnType.Name, " ve ", RightOperand.ReturnType.Name, " arasında '", OperatorType, "' işlemi uygulanamaz."), innerException);
            }
        }

        protected Type PutInstructionsAnd(IInstructionEventArgs e)
        {
            Label isFalse = e.Generator.DefineLabel();
            Label end = e.Generator.DefineLabel();

            if (LeftOperand.IsHandled())
            {
                e.Generator.Emit(OpCodes.Ldc_I4, 1);
            }
            else
            {
                LeftOperand.PutInstructions(e);
            }

            e.Generator.Emit(OpCodes.Brfalse, isFalse);

            if (RightOperand.IsHandled())
            {
                e.Generator.Emit(OpCodes.Ldc_I4, 1);
            }
            else
            {
                RightOperand.PutInstructions(e);
            }

            e.Generator.Emit(OpCodes.Br, end);
            e.Generator.MarkLabel(isFalse);
            e.Generator.Emit(OpCodes.Ldc_I4, 0);
            e.Generator.MarkLabel(end);

            return Types.Boolean;
        }

        protected Type PutInstructionsOr(IInstructionEventArgs e)
        {
            Label isTrue = e.Generator.DefineLabel();
            Label end = e.Generator.DefineLabel();

            LeftOperand.PutInstructions(e);
            e.Generator.Emit(OpCodes.Brtrue, isTrue);

            RightOperand.PutInstructions(e);
            e.Generator.Emit(OpCodes.Br, end);

            e.Generator.MarkLabel(isTrue);
            e.Generator.Emit(OpCodes.Ldc_I4, 1);
            e.Generator.MarkLabel(end);

            return Types.Boolean;
        }

        protected Type PutBinaryInstructions(IInstructionEventArgs e)
        {
            Type returnType;

            LeftLocal = e.Generator.DeclareLocal(LeftOperand.PutInstructions(e));
            LeftType = Type.GetTypeCode(LeftLocal.LocalType);

            e.Generator.Emit(OpCodes.Stloc, LeftLocal);

            RightLocal = e.Generator.DeclareLocal(RightOperand.PutInstructions(e));
            RightType = Type.GetTypeCode(RightLocal.LocalType);

            e.Generator.Emit(OpCodes.Stloc, RightLocal);

            if (RightType == TypeCode.Object)
            {
                if (MethodCache.IDataRecordSet.IsAssignableFrom(RightLocal.LocalType))
                {
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    e.Generator.Emit(OpCodes.Call, MethodCache.Data.ExecuteScalar.MakeGenericMethod(LeftLocal.LocalType));

                    RightType = LeftType;
                    RightLocal = e.Generator.DeclareLocal(LeftLocal.LocalType);
                    e.Generator.Emit(OpCodes.Stloc, RightLocal);
                }
            }

            if (PutTypeInstructions(e, out returnType))
            {
                PutOperatorInstructions(e, ref returnType);
            }

            return returnType;
        }

        protected void PutOperatorInstructions(IInstructionEventArgs e, ref Type returnType)
        {
            if (OperatorType == BinaryOperatorType.Equals)
            {
                e.Generator.Emit(OpCodes.Ceq);
                returnType = Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.NotEquals)
            {
                e.Generator.Emit(OpCodes.Ceq);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);
                returnType = Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Greater)
            {
                e.Generator.Emit(OpCodes.Cgt);
                returnType = Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.GreaterOrEqual)
            {
                e.Generator.Emit(OpCodes.Clt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);
                returnType = Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Less)
            {
                e.Generator.Emit(OpCodes.Clt);
                returnType = Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.LessOrEqual)
            {
                e.Generator.Emit(OpCodes.Cgt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);
                returnType = Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Add)
            {
                e.Generator.Emit(OpCodes.Add);
            }
            else if (OperatorType == BinaryOperatorType.Subtract)
            {
                e.Generator.Emit(OpCodes.Sub);
            }
            else if (OperatorType == BinaryOperatorType.Multiply)
            {
                e.Generator.Emit(OpCodes.Mul);
            }
            else if (OperatorType == BinaryOperatorType.Divide)
            {
                e.Generator.Emit(OpCodes.Div);
            }
            else if (OperatorType == BinaryOperatorType.Modulo)
            {
                e.Generator.Emit(OpCodes.Rem);
            }
            else if (OperatorType == BinaryOperatorType.BitwiseAnd)
            {
                e.Generator.Emit(OpCodes.And);
            }
            else if (OperatorType == BinaryOperatorType.BitwiseOr)
            {
                e.Generator.Emit(OpCodes.Or);
            }
            else if (OperatorType == BinaryOperatorType.BitwiseLeftShift)
            {
                e.Generator.Emit(OpCodes.Shl);
            }
            else if (OperatorType == BinaryOperatorType.BitwiseRightShift)
            {
                e.Generator.Emit(OpCodes.Shr);
            }
            else if (OperatorType == BinaryOperatorType.BitwiseXor)
            {
                e.Generator.Emit(OpCodes.Xor);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected bool PutTypeInstructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (LeftType)
            {
                case TypeCode.Boolean:
                case TypeCode.Int32:
                    return PutInt32Instructions(e, out returnType);

                case TypeCode.Int64:
                    return PutInt64Instructions(e, out returnType);

                case TypeCode.Single:
                    return PutSingleInstructions(e, out returnType);

                case TypeCode.Double:
                    return PutDoubleInstructions(e, out returnType);

                case TypeCode.Decimal:
                    return PutDecimalInstructions(e, out returnType);

                case TypeCode.String:
                    return PutStringInstructions(e, out returnType);

                case TypeCode.DateTime:
                    return PutDateTimeInstructions(e, out returnType);
            }

            throw new NotSupportedException();
        }

        protected bool PutInt32Instructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (RightType)
            {
                case TypeCode.Boolean:
                case TypeCode.Int32:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = Types.Int32;
                    return true;

                case TypeCode.Int64:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Conv_I8);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = Types.Int64;
                    return true;

                case TypeCode.Single:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Conv_R4);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = Types.Single;
                    return true;

                case TypeCode.Double:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Conv_R8);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = Types.Double;
                    return true;

                case TypeCode.Decimal:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    PutConvertToDecimalInstructions(e, LeftType);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = PutDecimalOperatorInstructions(e);
                    return false;
            }

            throw new NotSupportedException();
        }

        protected bool PutInt64Instructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (RightType)
            {
                case TypeCode.Int32:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    e.Generator.Emit(OpCodes.Conv_I8);
                    returnType = Types.Int64;
                    return true;
            }

            throw new NotSupportedException();
        }

        protected Type PutDecimalOperatorInstructions(IInstructionEventArgs e)
        {
            if (OperatorType == BinaryOperatorType.Equals)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.IsEquals);
                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.NotEquals)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.IsEquals);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Greater)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Cgt);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.GreaterOrEqual)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Clt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Less)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Clt);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.LessOrEqual)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Cgt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Add)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.Add);
            }
            else if (OperatorType == BinaryOperatorType.Multiply)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.Multiply);
            }
            else if (OperatorType == BinaryOperatorType.Divide)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.Decimal.Divide);
            }
            else
            {
                throw new NotSupportedException();
            }

            return Types.Decimal;
        }

        protected void PutConvertToDecimalInstructions(IInstructionEventArgs e, TypeCode valueType)
        {
            switch (valueType)
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                    e.Generator.Emit(OpCodes.Newobj, MethodCache.Decimal.ConstInt32);
                    break;

                case TypeCode.Int64:
                    e.Generator.Emit(OpCodes.Newobj, MethodCache.Decimal.ConstInt64);
                    break;

                case TypeCode.Single:
                    e.Generator.Emit(OpCodes.Newobj, MethodCache.Decimal.ConstSingle);
                    break;

                case TypeCode.Decimal:
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        protected bool PutDecimalInstructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (RightType)
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Decimal:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    PutConvertToDecimalInstructions(e, RightType);
                    returnType = PutDecimalOperatorInstructions(e);
                    return false;
            }

            throw new NotSupportedException();
        }

        protected bool PutSingleInstructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (RightType)
            {
                case TypeCode.Int32:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    e.Generator.Emit(OpCodes.Conv_R4);
                    returnType = Types.Single;
                    return true;
            }

            throw new NotSupportedException();
        }

        protected bool PutDoubleInstructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (RightType)
            {
                case TypeCode.Int32:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    e.Generator.Emit(OpCodes.Conv_R8);
                    returnType = Types.Double;
                    return true;

                case TypeCode.Double:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = Types.Double;
                    return true;
            }

            throw new NotSupportedException();
        }

        protected Type PutStringOperatorInstructions(IInstructionEventArgs e)
        {
            if (OperatorType == BinaryOperatorType.Equals)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.IsEquals);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Greater)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.CompareOrdinal);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Cgt);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.GreaterOrEqual)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.CompareOrdinal);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Clt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Less)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.CompareOrdinal);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Clt);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.LessOrEqual)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.CompareOrdinal);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Cgt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Like)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.Like);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.iLike)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.iLike);

                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Add)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.String.Concat);

                return Types.String;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected bool PutStringInstructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (RightType)
            {
                case TypeCode.String:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = PutStringOperatorInstructions(e);
                    return false;
            }

            throw new NotSupportedException();
        }

        protected Type PutDateTimeOperatorInstructions(IInstructionEventArgs e)
        {
            if (OperatorType == BinaryOperatorType.Equals)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.DateTime.IsEquals);
                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Greater)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.DateTime.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Cgt);
                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.GreaterOrEqual)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.DateTime.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Clt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);
                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.Less)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.DateTime.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Clt);
                return Types.Boolean;
            }
            else if (OperatorType == BinaryOperatorType.LessOrEqual)
            {
                e.Generator.Emit(OpCodes.Call, MethodCache.DateTime.Compare);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Cgt);
                e.Generator.Emit(OpCodes.Ldc_I4, 0);
                e.Generator.Emit(OpCodes.Ceq);
                return Types.Boolean;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected bool PutDateTimeInstructions(IInstructionEventArgs e, out Type returnType)
        {
            switch (RightType)
            {
                case TypeCode.DateTime:
                    e.Generator.Emit(OpCodes.Ldloc, LeftLocal);
                    e.Generator.Emit(OpCodes.Ldloc, RightLocal);
                    returnType = PutDateTimeOperatorInstructions(e);
                    return false;
            }

            throw new NotSupportedException();
        }

        static string ToSymbol(BinaryOperatorType operatorType)
        {
            switch (operatorType)
            {
                case BinaryOperatorType.And: return "And";
                case BinaryOperatorType.Or: return "Or";
                case BinaryOperatorType.Equals: return "=";
                case BinaryOperatorType.NotEquals: return "<>";
                case BinaryOperatorType.Greater: return ">";
                case BinaryOperatorType.GreaterOrEqual: return ">=";
                case BinaryOperatorType.Less: return "<";
                case BinaryOperatorType.LessOrEqual: return "<=";
                case BinaryOperatorType.Like: return "Like";
                case BinaryOperatorType.iLike: return "iLike";
                case BinaryOperatorType.LikeAll: return "LikeAll";
                case BinaryOperatorType.iLikeAll: return "iLikeAll";
                case BinaryOperatorType.Add: return "+";
                case BinaryOperatorType.Subtract: return "-";
                case BinaryOperatorType.Multiply: return "*";
                case BinaryOperatorType.Divide: return "/";
                case BinaryOperatorType.Modulo: return "%";
                case BinaryOperatorType.BitwiseAnd: return "&";
                case BinaryOperatorType.BitwiseOr: return "|";
                case BinaryOperatorType.BitwiseXor: return "^";
                case BinaryOperatorType.BitwiseLeftShift: return "<<";
                case BinaryOperatorType.BitwiseRightShift: return ">>";
            }

            return string.Empty;
        }

        public static IExpressionOperator Combine(BinaryOperatorType operatorType, IExpressionOperator leftOperand, IExpressionOperator rightOperand)
        {
            if (leftOperand.HasValue() && rightOperand.HasValue())
            {
                var leftBinary = leftOperand as BinaryOperator;
                var rightBinary = rightOperand as BinaryOperator;

                if (leftBinary.HasValue() && !leftBinary.IsGroup)
                {
                    if (leftBinary.OperatorType == BinaryOperatorType.And || leftBinary.OperatorType == BinaryOperatorType.Or)
                    {
                        return new BinaryOperator(leftBinary.OperatorType, leftBinary.LeftOperand, Combine(operatorType, leftBinary.RightOperand, rightOperand));
                    }
                    else if (BinaryOperatorTypeComparer.Compare(operatorType, leftBinary.OperatorType) > 0)
                    {
                        return new BinaryOperator(leftBinary.OperatorType, leftBinary.LeftOperand, new BinaryOperator(operatorType, leftBinary.RightOperand, rightOperand));
                    }
                }
                else if (rightBinary.HasValue() && !rightBinary.IsGroup)
                {
                    if (BinaryOperatorTypeComparer.Compare(operatorType, rightBinary.OperatorType) > 0)
                    {
                        return new BinaryOperator(leftBinary.OperatorType, leftBinary.LeftOperand, new BinaryOperator(operatorType, leftBinary.RightOperand, rightOperand));
                    }

                    return rightBinary;
                }

                return new BinaryOperator(operatorType, leftOperand, rightOperand);
            }

            return leftOperand.HasValue() ? leftOperand : rightOperand;
        }

        internal static bool TryParseOperator(ITokenEnumerator e, out BinaryOperatorType operatorType)
        {
            Token current = e.Current;
            operatorType = BinaryOperatorType.None;

            if (current is CharToken)
            {
                switch ((char)current.Value)
                {
                    case '=': operatorType = BinaryOperatorType.Equals; break;
                    case '+': operatorType = BinaryOperatorType.Add; break;
                    case '-': operatorType = BinaryOperatorType.Subtract; break;
                    case '*': operatorType = BinaryOperatorType.Multiply; break;
                    case '/': operatorType = BinaryOperatorType.Divide; break;
                    case '%': operatorType = BinaryOperatorType.Modulo; break;
                    case '&': operatorType = BinaryOperatorType.BitwiseAnd; break;
                    case '|': operatorType = BinaryOperatorType.BitwiseOr; break;
                    case '^': operatorType = BinaryOperatorType.BitwiseXor; break;
                }

                if (current.Equals('<'))
                {
                    if (e.NextIs('>'))
                    {
                        e.MoveNext();
                        operatorType = BinaryOperatorType.NotEquals;
                    }
                    else if (e.NextIs('='))
                    {
                        e.MoveNext();
                        operatorType = BinaryOperatorType.LessOrEqual;
                    }
                    else if (e.NextIs('<'))
                    {
                        e.MoveNext();
                        operatorType = BinaryOperatorType.BitwiseLeftShift;
                    }
                    else
                    {
                        operatorType = BinaryOperatorType.Less;
                    }
                }
                else if (current.Equals('>'))
                {
                    if (e.NextIs('='))
                    {
                        e.MoveNext();
                        operatorType = BinaryOperatorType.GreaterOrEqual;
                    }
                    else if (e.NextIs('>'))
                    {
                        e.MoveNext();
                        operatorType = BinaryOperatorType.BitwiseRightShift;
                    }
                    else
                    {
                        operatorType = BinaryOperatorType.Greater;
                    }
                }
            }

            if (operatorType == BinaryOperatorType.None)
            {
                if (current.Equals("And"))
                {
                    operatorType = BinaryOperatorType.And;
                }
                else if (current.Equals("Or"))
                {
                    operatorType = BinaryOperatorType.Or;
                }
                else if (current.Equals("Like"))
                {
                    operatorType = BinaryOperatorType.Like;
                }
                else if (current.Equals("iLike"))
                {
                    operatorType = BinaryOperatorType.iLike;
                }
                else if (current.Equals("LikeAll"))
                {
                    operatorType = BinaryOperatorType.LikeAll;
                }
                else if (current.Equals("iLikeAll"))
                {
                    operatorType = BinaryOperatorType.iLikeAll;
                }
            }

            if (operatorType != BinaryOperatorType.None)
            {
                if (e.MoveNext() && e.Current is CharToken && !e.Current.Any('(', '@'))
                {
                    throw new InvalidOperatorException(string.Concat(current, e.Current));
                }

                return true;
            }

            return false;
        }

        protected override string OnLegacyToString(ILegacyToStringVisitor visitor)
        {
            if (visitor.IsNull())
            {
                string s = string.Concat(LeftOperand.LegacyToString(visitor), ' ', OperatorSymbol, ' ', RightOperand.LegacyToString(visitor));

                if (IsGroup)
                {
                    return string.Concat('(', s, ')');
                }

                return s;
            }

            return visitor.LegacyToString(this);
        }

        #region IValueOperand

        bool IValueOperand.SupportsArguments
        {
            get { return RightOperand is IValueOperand; }
        }

        Quotes IValueOperand.Quote
        {
            get { return (RightOperand as IValueOperand).Quote; }
            set { (RightOperand as IValueOperand).Quote = value; }
        }

        void IValueOperand.Add(string name)
        {
            (RightOperand as IValueOperand).Add(name);
        }

        void IValueOperand.AddWithArgument(string name, IExpressionOperator arg)
        {
            (RightOperand as IValueOperand).AddWithArgument(name, arg);
        }

        void IValueOperand.AddWithArguments(string name, IExpressionOperatorCollection args)
        {
            (RightOperand as IValueOperand).AddWithArguments(name, args);
        }

        IList<IExpressionOperatorCollection> IValueOperand.Arguments
        {
            get { return (RightOperand as IValueOperand).Arguments; }
        }

        object IValueOperand.Value
        {
            get { return (RightOperand as IValueOperand).Value; }
            set { (RightOperand as IValueOperand).Value = value; }
        }

        string IValueOperand.ToString()
        {
            return (RightOperand as IValueOperand).ToString();
        }

        #endregion

        public override IEnumerator<IExpressionOperator> GetEnumerator()
        {
            yield return LeftOperand;
            yield return RightOperand;
        }
    }
}