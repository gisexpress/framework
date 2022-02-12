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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    public abstract class ApplicationComponent<TValue> : IApplicationComponent, IApplicationComponentDesignerListener where TValue : ISupportInitialize
    {
        protected ApplicationComponent(IApplicationComponentDesigner designer, TValue value)
        {
            Designer = designer;
            Value = value;
            AllowAlignments = true;
            AllowProperties = true;
            Properties = new Hashtable();
            PaintArgs = new ApplicationComponentPaintEventArgs(designer, this);
            ValidatingArgs = new ApplicationComponentEventArgs(designer, this);
            ValidatingHandlers = new HashSet<ApplicationComponentEventHandler>();
            PropertyDescriptors = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
        }

        protected bool Started;
        protected bool Detached;
        protected bool MouseOver;
        protected bool IsDisposed;
        protected readonly HashSet<ApplicationComponentEventHandler> ValidatingHandlers;

        protected readonly ApplicationComponentPaintEventArgs PaintArgs;
        protected readonly ApplicationComponentEventArgs ValidatingArgs;
        protected readonly PropertyDescriptorCollection PropertyDescriptors;

        public virtual event ApplicationComponentPaintEventHandler Paint;

        public virtual event ApplicationComponentEventHandler Validating
        {
            add { ValidatingHandlers.Add(value); }
            remove { ValidatingHandlers.Remove(value); }
        }

        public virtual event ApplicationComponentEditCompletedEventHandler EditCompleted;

        public int Priority
        {
            get { return 1; }
        }

        public virtual bool IsEmpty()
        {
            return false;
        }

        public virtual bool IsBusy()
        {
            return false;
        }

        public virtual bool IsDetached()
        {
            return Detached;
        }

        public bool IsStartedEditing()
        {
            return Started;
        }

        public virtual bool AllowAlignments
        {
            get;
            set;
        }

        public virtual bool AllowProperties
        {
            get;
            set;
        }

        public virtual bool AllowBeginValidation
        {
            get { return true; }
        }

        public Cursor Cursor
        {
            get
            {
                if (ValidatingArgs.Cancel)
                {
                    return ApplicationAppereance.CursorCancel;
                }

                return ApplicationAppereance.CursorCross;
            }
        }

        public IApplicationComponentDesigner Designer
        {
            get;
            protected set;
        }

        public ICustomTypeDescriptor Descriptor
        {
            get;
            protected set;
        }

        public IApplicationComponent Parent
        {
            get;
            protected set;
        }

        public virtual object ValueOwner
        {
            get { return Value; }
        }

        public TValue Value
        {
            get;
            set;
        }

        public Hashtable Properties
        {
            get;
            protected set;
        }

        protected virtual void OnBeginEdit()
        {
            Value.BeginInit();
        }

        protected virtual void OnEndEdit(ComponentEditCompleteAction action)
        {
            using (this)
            {
                if (action != ComponentEditCompleteAction.Delete)
                {
                    Value.EndInit();
                }

                OnEditCompleted(action);
            }
        }

        protected virtual void OnDelete()
        {
            OnEndEdit(ComponentEditCompleteAction.Delete);
        }

        protected virtual void OnPaint(PaintEventArgs e)
        {
            PaintArgs.Reset(e.Graphics);
            Paint.InvokeSafely(PaintArgs);
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    OnDelete();
                    break;
            }
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
        }

        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseOver = true;
            Designer.Flush();
        }

        protected virtual void OnMouseLeave(EventArgs e)
        {
            MouseOver = false;
            Designer.Flush();
        }

        protected virtual void OnMouseDown(MouseEventArgs e)
        {
        }

        protected virtual void OnMouseUp(MouseEventArgs e)
        {
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseOver = true;
        }

        protected virtual void OnEditCompleted(ComponentEditCompleteAction action)
        {
            EditCompleted.InvokeSafely<ApplicationComponentEditCompletedEventArgs>(this, action);
        }

        public void Validate()
        {
            OnValidate();
        }

        protected virtual void OnValidate()
        {
            var c = ValidatingArgs.Cancel;

            ValidatingArgs.Reset();
            ValidatingHandlers.OrderByDescending(e => e.Target).ForEach(e => e(ValidatingArgs));

            if (!c.Equals(ValidatingArgs.Cancel))
            {
                Designer.UpdateCursor();
            }
        }

        #region IEditableComponent

        ICustomTypeDescriptor IApplicationComponent.TypeDescriptor
        {
            get { return Descriptor; }
            set { Descriptor = value; }
        }

        IApplicationComponent IApplicationComponent.Parent
        {
            get { return Parent; }
            set { Parent = value; }
        }

        object IApplicationComponent.Value
        {
            get { return Value; }
        }

        IApplicationComponentDesignerListener IApplicationComponent.Listener
        {
            get { return this; }
        }

        void IApplicationComponent.BeginEdit()
        {
            OnBeginEdit();
        }

        void IApplicationComponent.EndEdit(ComponentEditCompleteAction action)
        {
            OnEndEdit(action);
        }

        void IApplicationComponentDesignerListener.Paint(PaintEventArgs e)
        {
            OnPaint(e);
        }

        void IApplicationComponentDesignerListener.KeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        void IApplicationComponentDesignerListener.KeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        void IApplicationComponentDesignerListener.MouseEnter(EventArgs e)
        {
            OnMouseEnter(e);
        }

        void IApplicationComponentDesignerListener.MouseLeave(EventArgs e)
        {
            OnMouseLeave(e);
        }

        void IApplicationComponentDesignerListener.MouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        void IApplicationComponentDesignerListener.MouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        void IApplicationComponentDesignerListener.MouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        #endregion

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

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return GetProperties(default(Attribute[]));
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return GetProperties(attributes);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return GetPropertyOwner(pd);
        }

        protected virtual object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        protected virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptors.Clear();
            AddProperties(PropertyDescriptors);
            return PropertyDescriptors;
        }

        public virtual void AddProperties(PropertyDescriptorCollection properties)
        {
        }

        #endregion

        public int CompareTo(object other)
        {
            return CompareTo(other as IApplicationComponentValidator);
        }

        public int CompareTo(IApplicationComponentValidator other)
        {
            return Priority.CompareTo(other.HasValue() ? other.Priority : int.MaxValue);
        }

        protected virtual void OnDispose()
        {
            Paint = null;
            EditCompleted = null;
            ValidatingHandlers.Clear();
            Properties.Clear();
            IsDisposed = true;
        }

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }
    }
}
