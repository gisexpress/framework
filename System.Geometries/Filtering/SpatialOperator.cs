//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright © GISExpress 2015-2022. All Rights Reserved.
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
using System.Runtime;

namespace System.Geometries.Filtering
{
    [CLSCompliant(false)]
    public class SpatialOperator : ExpressionOperator
    {
        public SpatialOperator(IEnvelope bounds)
        {
            OperatorType = SpatialOperatorType.Intersects;
            Geometry = OperandFactory.Default.CreateValueOperand(bounds);
        }

        public override bool IsEmpty()
        {
            return Geometry.IsNull() || Geometry.IsEmpty();
        }

        protected IOperandProperty Operand;
        protected SpatialOperatorType OperatorTypeValue;
        protected IOperandProperty PropertyValue;
        protected IExpressionOperator GeometryValue;

        public string OperatorName
        {
            get { return string.Concat("Is", Enums.GetName(OperatorType)); }
        }

        public SpatialOperatorType OperatorType
        {
            get { return OperatorTypeValue; }
            set
            {
                if (OperatorTypeValue == value)
                {
                    return;
                }

                OperatorTypeValue = value;
                Operand = default;
            }
        }

        public IOperandProperty Property
        {
            get { return PropertyValue ?? OperandFactory.Default.CreateOperandProperty(Constants.Data.Shape); }
            set
            {
                PropertyValue = value;
                Operand = default;
            }
        }

        public IExpressionOperator Geometry
        {
            get { return GeometryValue; }
            set
            {
                GeometryValue = value;
                Operand = default;
            }
        }

        protected IValueOperand ToOperand()
        {
            if (Operand == null)
            {
                Operand = Property;
                Operand.AddWithArgument(OperatorName, Geometry);
            }

            return Operand;
        }

        protected override Type OnPutInstructions(IInstructionEventArgs e)
        {
            return ToOperand().PutInstructions(e);
        }

        protected override string OnLegacyToString(ILegacyToStringVisitor visitor)
        {
            IValueOperand operand = ToOperand();

            try
            {
                return operand.LegacyToString(visitor);
            }
            finally
            {
                Handled = operand.Handled;
            }
        }

        public override IEnumerator<IExpressionOperator> GetEnumerator()
        {
            return ToOperand().GetEnumerator();
        }
    }
}
