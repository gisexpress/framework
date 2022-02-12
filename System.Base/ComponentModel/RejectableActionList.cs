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
using System.Collections.ObjectModel;
using System.Linq;

namespace System.ComponentModel
{
    public delegate void ActionEventHandler<T>(T oldValue, T newValue);

    public class RejectableActionList : Collection<IRejectableAction>
    {
        public RejectableActionList()
            : this(true)
        {
        }

        public RejectableActionList(bool ignoreDuplicateActions)
        {
            IgnoreDuplicateActions = ignoreDuplicateActions;
        }

        public bool IgnoreDuplicateActions
        {
            get;
            set;
        }

        public void Add<T>(int id, ActionEventHandler<T> action, T oldValue, T newValue)
        {
            var item = new RejectableAction<T>(id, action, oldValue, newValue);
            item.Accept();
            base.Add(item);
        }

        public void Accept()
        {
            foreach (IRejectableAction item in this)
            {
                item.Accept();
            }

            Clear();
        }

        public void Reject()
        {
            foreach (IRejectableAction item in this)
            {
                item.Reject();
            }

            Clear();
        }

        protected override void InsertItem(int index, IRejectableAction item)
        {
            if (IgnoreDuplicateActions)
            {
                var oldItem = this.Cast<IRejectableAction>().FirstOrDefault(o => o.Id == item.Id);

                if (oldItem.HasValue())
                {
                    oldItem.NewValue = item.NewValue;
                    return;
                }
            }

            base.InsertItem(index, item);
        }

        protected class RejectableAction<T> : IRejectableAction
        {
            public RejectableAction(int id, ActionEventHandler<T> action, T oldValue, T newValue)
            {
                Id = id;
                Action = action;
                OldValue = oldValue;
                NewValue = newValue;
            }

            protected enum ActionState
            {
                None = 0,
                Accepted,
                Rejected
            }

            protected ActionState State
            {
                get;
                set;
            }

            public int Id
            {
                get;
                set;
            }

            public ActionEventHandler<T> Action
            {
                get;
                set;
            }

            public T OldValue
            {
                get;
                set;
            }

            public T NewValue
            {
                get;
                set;
            }

            object IRejectableAction.OldValue
            {
                get { return OldValue; }
                set { OldValue = (T)value; }
            }

            object IRejectableAction.NewValue
            {
                get { return NewValue; }
                set { NewValue = (T)value; }
            }

            public void Accept()
            {
                if (State != ActionState.Accepted)
                {
                    Action(OldValue, NewValue);
                    State = ActionState.Accepted;
                }
            }

            public void Reject()
            {
                if (State != ActionState.Rejected)
                {
                    Action(NewValue, OldValue);
                    State = ActionState.Rejected;
                }
            }
        }
    }

    public interface IRejectableAction
    {
        int Id
        {
            get;
        }

        object OldValue
        {
            get;
            set;
        }

        object NewValue
        {
            get;
            set;
        }

        void Accept();

        void Reject();
    }
}
