﻿//////////////////////////////////////////////////////////////////////////////////////////////////
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
using System.Linq;
using System.Runtime;

namespace System.Data
{
    public class DataSource : IDataTable
    {
        public DataSource(IDataSet dataSet, string name)
        {
            DataSet = dataSet;
            Name = name;
            Prefix = string.Concat(Name, '.');
        }

        protected readonly string Prefix;
        protected MemberAccessorCollection Members;

        public string Name
        {
            get;
            protected set;
        }

        public IDataSet DataSet
        {
            get;
            protected set;
        }

        public MemberAccessorCollection Properties
        {
            get { return Members ?? (Members = GetProperties()); }
        }

        public bool IsMember(string name)
        {
            return Properties[name].HasValue();
        }

        public bool GetFieldNameWithoutPrefix(string name, out string value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name.Contains(".") && name.StartsWithIgnoreCase(Prefix))
                {
                    return IsMember(value = name.Remove(0, Prefix.Length));
                }
            }

            value = default(string);
            return false;
        }

        public virtual IEnumerable<IDataRecord> GetRows()
        {
            return DataSet.GetRows(Name);
        }

        public virtual MemberAccessorCollection GetProperties()
        {
            MemberAccessorCollection members = DataSet.GetProperties(Name);

            if (members.IsNull())
            {
                return MemberAccessor.GetMemberAccessors(GetRows().First().GetType());
            }

            return members;
        }

        public override string ToString()
        {
            return "Name: {0}".FormatInvariant(Name);
        }
    }
}