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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace System.IO
{
    [DebuggerDisplay("{ToString()}")]
    public class TokenCollection : Collection<Token>, ITokenEnumerator
    {
        protected bool Flag;
        protected Exception Error;
        protected int Position = -1;

        public object Component
        {
            get;
            set;
        }

        public Exception Exception
        {
            get { return Error; }
            set { Error = Error ?? value; }
        }

        public virtual bool CanRead
        {
            get { return Flag; }
        }

        public void Reset()
        {
            Position = -1;
        }

        public bool MoveNext()
        {
            return MoveNext(false);
        }

        public bool MoveNext(bool throwIfFail)
        {
            if (Position + 1 < Count)
            {
                Position++;
                Flag = true;
                Current = this[Position];
                return true;
            }

            if (throwIfFail)
            {
                throw SyntaxError();
            }

            Flag = false;
            return false;
        }

        public Token Current
        {
            get;
            protected set;
        }

        public bool ReadNext(char c)
        {
            return MoveNext() && Current.Equals(c);
        }

        public bool ReadNext(string s)
        {
            return MoveNext() && Current.Equals(s);
        }

        public bool ReadWord(out WordToken token)
        {
            if (MoveNext())
            {
                return (token = Current as WordToken).HasValue();
            }

            token = null;
            return false;
        }

        public bool ReadQuote(out QuoteToken token)
        {
            if (MoveNext())
            {
                return (token = Current as QuoteToken).HasValue();
            }

            token = null;
            return false;
        }

        public bool ReadNumber(out NumberToken token)
        {
            if (MoveNext())
            {
                return (token = Current as NumberToken).HasValue();
            }

            token = null;
            return false;
        }

        public bool ReadString(out string value, params object[] terminationArgs)
        {
            value = default(string);

            while (true)
            {
                if (Current.Any(terminationArgs))
                {
                    return value.HasValue();
                }

                value = string.Concat(value, Current.Value);

                if (!MoveNext())
                {
                    break;
                }
            }

            return value.HasValue();
        }

        public bool NextIs(object token)
        {
            if (Position + 1 < Count)
            {
                return this[Position + 1].Equals(token);
            }

            return false;
        }

        public bool NextIsAny(params object[] tokens)
        {
            if (Position + 1 < Count)
            {
                return this[Position + 1].Any(tokens);
            }

            return false;
        }

        public bool Equals(params object[] tokens)
        {
            bool r1 = true, r2;
            IEnumerator<Token> e1 = GetEnumerator();
            IEnumerator e2 = tokens.GetEnumerator();

            while (r1)
            {
                r1 = e1.MoveNext();
                r2 = e2.MoveNext();

                if (r1 != r2 || (r1 && !e1.Current.Equals(e2.Current)))
                {
                    return false;
                }
            }

            return true;
        }

        public Exception SyntaxError()
        {
            return new SyntaxErrorException("IncorrectSyntax".FormatInvariant(Current.Value));
        }

        public string ReadToEnd()
        {
            return ReadToEnd(char.MinValue);
        }

        public string ReadToEnd(char c)
        {
            var value = string.Empty;

            while (CanRead)
            {
                value = string.Concat(value, Current.LegacyToString());

                if (MoveNext())
                {
                    if (Current.Equals(c))
                    {
                        MoveNext();
                        break;
                    }

                    continue;
                }
                else
                {
                    break;
                }
            }

            return value;
        }

        public virtual string LegacyToString()
        {
            return this.ToArray().Join(string.Empty);
        }

        public override string ToString()
        {
            return string.Concat("Count: ", Count, ", Current: ", Current);
        }
    }
}