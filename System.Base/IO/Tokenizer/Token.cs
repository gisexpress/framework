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

using System.Globalization;

namespace System.IO
{
    public abstract class Token
    {
        protected Token(int line, int position, object value)
        {
            Line = line;
            Position = position;
            Value = value;
        }

        public int Line
        {
            get;
            protected set;
        }

        public int Position
        {
            get;
            protected set;
        }

        public object Value
        {
            get;
            protected set;
        }

        public virtual string StringValue
        {
            get { return (string)Value; }
        }

        public override int GetHashCode()
        {
            return GetHashCode(Value);
        }

        public static int GetHashCode(object obj)
        {
            if (obj is Token)
            {
                return obj.GetHashCode();
            }

            return (Convert.ToString(obj, CultureInfo.InvariantCulture) ?? string.Empty).ToUpperInvariant().GetHashCode();
        }

        public bool Any(params object[] args)
        {
            foreach (object obj in args)
            {
                if (Equals(obj))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(GetHashCode(Value), GetHashCode(obj));
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }

        public abstract string LegacyToString();
    }
}
