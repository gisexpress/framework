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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace System.Data
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class DataProviderAttribute : Attribute
    {
        static DataProviderAttribute()
        {
            Values = new SortedDictionary<Guid, object>();
        }

        public DataProviderAttribute()
        {
            Enabled = true;
        }

        protected static readonly SortedDictionary<Guid, object> Values;

        [Browsable(false)]
        public abstract Type ProviderType
        {
            get;
        }

        [Browsable(false)]
        public virtual bool Browsable
        {
            get { return false; }
        }

        [Browsable(false)]
        public virtual string InvariantName
        {
            get { return default(string); }
        }

        public abstract IEnumerable<string> GetNames();

        [LocalizedDisplayName("Enabled")]
        public bool Enabled
        {
            get;
            set;
        }

        [Browsable(false)]
        public virtual bool IsNative
        {
            get { return false; }
        }

        [LocalizedDisplayName("DisplayName")]
        public virtual string DisplayName
        {
            get { return Localize("DisplayName"); }
        }

        [Browsable(false)]
        public virtual string Description
        {
            get { return Localize("Description"); }
        }

        [LocalizedDisplayName("Author")]
        public string Author
        {
            get { return ProviderType.Assembly.GetAttribute<AssemblyCompanyAttribute>().Company; }
        }

        [LocalizedDisplayName("Version")]
        public string Version
        {
            get { return ProviderType.Assembly.GetAttribute<AssemblyFileVersionAttribute>().Version; }
        }

        [Browsable(false)]
        public virtual Image Image
        {
            get { return default(Image); }
        }

        [Browsable(false)]
        public override object TypeId
        {
            get { return base.TypeId; }
        }

        [Browsable(false)]
        public object Instance
        {
            get
            {
                object value;

                lock (Values)
                {
                    if (Values.TryGetValue(ProviderType.GUID, out value))
                    {
                        return value;
                    }

                    Values.Add(ProviderType.GUID, value = Activator.CreateInstance(ProviderType, true));
                }

                return value;
            }
        }

        public virtual Control CreateConnectionControl()
        {
            return default(Control);
        }

        protected string Localize(string key)
        {
            return Localization.Localize("DataProvider({0}).{1}".FormatInvariant(ProviderType.GUID, key));
        }
    }
}
