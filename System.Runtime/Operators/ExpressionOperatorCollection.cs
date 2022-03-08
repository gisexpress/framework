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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace System.Runtime
{
    internal class ExpressionOperatorCollection : KeyedCollection<string, IExpressionOperator>, IExpressionOperatorCollection
    {
        public ExpressionOperatorCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
            AllowMultiple = true;
        }

        public ExpressionOperatorCollection(IExpressionOperator operand)
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
            Operand = operand;
            AllowMultiple = true;
        }

        public ExpressionOperatorCollection(IExpressionOperator operand, IList<object> literals)
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
            Operand = operand;
            AllowMultiple = true;

            foreach (object value in literals)
            {
                Add(value as IExpressionOperator ?? OperandFactory.Default.CreateValueOperand(value));
            }
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public bool AllowMultiple
        {
            get;
            set;
        }

        public IExpressionOperator Operand
        {
            get;
            protected set;
        }

        public IExpressionOperator Find(string name)
        {
            return Items.FirstOrDefault(o => o.Name.EqualsIgnoreCase(name));
        }

        public IExpressionOperator Dequeue()
        {
            IExpressionOperator o = Items.First();

            if (Remove(o))
            {
                return o;
            }

            throw new InvalidOperationException();
        }

        internal static bool TryParseOperator(ITokenEnumerator e, out IExpressionOperator o)
        {
            if (e.Current.Equals('('))
            {
                return OperandFactory.Default.CreateOperatorCollection().Read(e, out o);
            }

            o = default(IExpressionOperator);
            return false;
        }

        public bool Read(ITokenEnumerator e)
        {
            IExpressionOperator o;
            return Read(e, out o);
        }

        public bool Read(ITokenEnumerator e, out IExpressionOperator o)
        {
            if (e.Current.Equals('('))
            {
                if (e.MoveNext())
                {
                    while (e.CanRead)
                    {
                        o = ExpressionOperator.Read(e, this);

                        Add(o);

                        if (e.Current.Equals(')'))
                        {
                            e.MoveNext();
                            break;
                        }

                        if (AllowMultiple && e.Current.Equals(','))
                        {
                            e.MoveNext();
                        }
                    }
                }

                if (Count > 0)
                {
                    o = Items.First();
                    return true;
                }
            }

            o = default(IExpressionOperator);
            return false;
        }

        public Type[] PutInstructions(IInstructionEventArgs e)
        {
            return OnPutInstructions(e).ToArray();
        }

        protected virtual IEnumerable<Type> OnPutInstructions(IInstructionEventArgs e)
        {
            foreach (IExpressionOperator o in Items)
            {
                yield return o.PutInstructions(e);
            }
        }

        protected override void RemoveItem(int index)
        {
            IExpressionOperator o = Items[index];

            base.RemoveItem(index);

            o.Owner = null;
            o.Name = null;
        }

        protected override void InsertItem(int index, IExpressionOperator item)
        {
            item.Owner = this;

            if (AllowMultiple)
            {
                if (string.IsNullOrEmpty(item.Name) || Contains(item.Name))
                {
                    item.Name = Items.GetName(string.Empty, string.Empty);
                }
            }
            else
            {
                item.Name = string.Empty;
            }

            if (!AllowMultiple && Count > 0)
            {
                throw new ExpressionParserException("Expecting an operator between '{0}' and '{1}'".FormatInvariant(this[0], item));
            }

            base.InsertItem(index, item);
        }

        protected override string GetKeyForItem(IExpressionOperator item)
        {
            return item.Name;
        }

        IEnumerable<string> GetEnumerator(ILegacyToStringVisitor visitor)
        {
            foreach (ExpressionOperator o in this)
            {
                yield return o.LegacyToString(visitor);
            }
        }

        public virtual string LegacyToString(ILegacyToStringVisitor visitor)
        {
            return GetEnumerator(visitor).Join(", ");
        }

        public override string ToString()
        {
            return LegacyToString(default(ILegacyToStringVisitor));
        }

        public IExpressionOperatorCollection Clone()
        {
            var c = new ExpressionOperatorCollection();

            foreach (IExpressionOperator o in Items)
            {
                c.Add(o.Clone());
            }

            return c;
        }
    }
}
