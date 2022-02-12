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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.ComponentModel
{
    [Guid("8AC4EAE4-68E8-4320-9FC8-1DBC1782AD9F")]
    public class PropertyNodeCollection : KeyedCollection<string, IPropertyDescriptor>, IPropertyCollection
    {
        public PropertyNodeCollection()
        {
        }

        public PropertyNodeCollection(IPropertyDescriptor parent)
        {
            Parent = parent;
        }

        public event Action<IPropertyDescriptor> PropertyValueChanged;

        protected readonly IPropertyDescriptor Parent;
        protected PropertyDescriptorCollection Properties;

        [Browsable(false)]
        protected IPropertyCollection Collection
        {
            get { return this; }
        }

        public bool IsNull(string name)
        {
            return Collection[name].IsNullOrDBNull();
        }

        public IPropertyDescriptor Add(string name)
        {
            return Add(name, default(string), typeof(IPropertyCollection), null);
        }

        public IPropertyDescriptor Add(string name, Type type)
        {
            return Add(name, default(string), type, null);
        }

        public IPropertyDescriptor Add(string name, object value)
        {
            return Add(name, default(string), default(Type), value);
        }

        public IPropertyDescriptor Add(string name, Type type, string category)
        {
            return Add(name, category, type, null);
        }

        public IPropertyDescriptor Add(string name, object value, string category)
        {
            return Add(name, category, default(Type), value);
        }

        IPropertyDescriptor Add(string name, string category, Type type, object value)
        {
            if (type.HasValue() || value.HasValue())
            {
                var item = new PropertyNode(name, type ?? value.GetType());

                item.Property.Category = category;
                item.Property.Value = value;

                Add(item);

                return item;
            }

            return default(IPropertyDescriptor);
        }

        public IPropertyDescriptor Find(string name, bool searchAllChildren)
        {
            IPropertyDescriptor item = this[name];

            if (searchAllChildren && item.IsNull())
            {
                item = Find(name, this);
            }

            return item;
        }

        IPropertyDescriptor Find(string name, IPropertyCollection properties)
        {
            foreach (PropertyNode item in properties)
            {
                IPropertyDescriptor property = item[name] ?? Find(name, item.Property.ChildProperties);

                if (property.HasValue())
                {
                    return property;
                }
            }

            return null;
        }

        public IEnumerable<IPropertyDescriptor> FindByType(Type propertyType)
        {
            var properties = new Stack<IPropertyDescriptor>(this);

            while (properties.Any())
            {
                IPropertyDescriptor property = properties.Pop();

                if (propertyType.IsAssignableFrom(property.PropertyType))
                {
                    yield return property;
                }

                foreach (var item in property.ChildProperties)
                {
                    properties.Push(item);
                }
            }
        }

        public new IPropertyDescriptor this[int index]
        {
            get { return base[index]; }
        }

        public new IPropertyDescriptor this[string name]
        {
            get
            {
                if (name.HasValue() && Contains(name))
                {
                    return base[name];
                }

                return default(IPropertyDescriptor);
            }
        }

        [Browsable(false)]
        protected new PropertyDescriptorCollection Items
        {
            get { return Properties ?? (Properties = new PropertyDescriptorCollection(base.Items.Cast<PropertyDescriptor>().ToArray())); }
        }

        public T GetValue<T>(string name)
        {
            return GetValue(name, default(T));
        }

        public T GetValue<T>(string name, T defaultValue)
        {
            return (T)(GetValue(name) ?? defaultValue);
        }

        public object GetValue(string name)
        {
            IPropertyDescriptor property = Find(name, true);

            if (property == null)
            {
                return null;
            }

            return property.Value;
        }

        public int IndexOf(string name)
        {
            return IndexOf(base[name]);
        }

        public void RaiseValueChanged(IPropertyDescriptor property)
        {
            PropertyValueChanged.InvokeSafely(property);
        }

        protected override string GetKeyForItem(IPropertyDescriptor item)
        {
            return item.Name;
        }

        protected override void ClearItems()
        {
            Properties = null;
            base.ClearItems();
        }

        protected override void InsertItem(int index, IPropertyDescriptor item)
        {
            Properties = null;

            IPropertyDescriptor property = Find(item.Name, false);

            if (property.HasValue())
            {
                property.Owner = this;
                property.Parent = Parent;
                //property.Category = item.Category;//todo
                property.DisplayName = item.FullName;
                property.Value = item.Value;
            }
            else
            {
                item.Owner = this;
                item.Parent = Parent;
                //item.Property.Category = item.Category;//todo
                item.DisplayName = item.FullName;

                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            Properties = null;
            base.RemoveItem(index);
        }

        #region ICustomTypeDescriptor

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return null;
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return null;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return Items;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return Items;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor e)
        {
            var item = Find(e.Name, true);

            if (item.HasValue())
            {
                return item.Parent;
            }

            return this;
        }

        #endregion

        public void LoadXml(XmlNode node)
        {
            //foreach (XmlNode node in node.Select("Property"))
            //{
            //    var item = new PropertyNode();
            //    item.LoadXml(node);
            //    Add(item);
            //}
        }

        public void SaveXml(XmlNode node)
        {
            //if (Count > 0)
            //{
            //    writer.WriteStartElement("properties");

            //    foreach (PropertyNode prop in this)
            //    {
            //        prop.WriteXml(writer);
            //    }

            //    writer.WriteEndElement();
            //}
        }

        IEnumerator<IPropertyDescriptor> IEnumerable<IPropertyDescriptor>.GetEnumerator()
        {
            if (Properties.HasValue())
            {
                foreach (IPropertyDescriptor item in this)
                {
                    yield return item;
                }
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public void Dispose()
        {
            PropertyValueChanged = null;
            Properties = null;
            GC.SuppressFinalize(this);
        }
    }
}
