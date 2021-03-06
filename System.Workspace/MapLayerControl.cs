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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace System.Workspace
{
    [ToolboxItem(false)]
    public class MapLayerControl : UserControl
    {
        public MapLayerControl()
        {
            Font = ApplicationAppereance.Fonts.DefaultFont;
        }

        protected Form Owner;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual MapWorkspace Workspace
        {
            get;
            set;
        }

        public void CreateLayers(MapLayer parent)
        {
            OnCreateLayers(parent);
        }

        protected virtual Exception OnValidate()
        {
            return default(Exception);
        }

        protected virtual void OnCreateLayers(MapLayer parent)
        {
        }

        public void ExecuteCommand(string command, params object[] args)
        {
            OnExecuteCommand(command, args);
        }

        protected virtual void OnExecuteCommand(string command, params object[] args)
        {
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Owner.HasValue())
            {
                Owner.FormClosing -= OnFormClosing;
            }

            Owner = FindForm();

            if (Owner.HasValue())
            {
                Owner.FormClosing += OnFormClosing;
            }

            base.OnParentChanged(e);
        }

        protected virtual void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (Owner.DialogResult == DialogResult.OK)
            {
                Exception error;

                if ((error = OnValidate()).HasValue())
                {
                    e.Cancel = true;
                    PopupMessage.Show(Owner.AcceptButton as Control, error.Message, MessageBoxIcon.Error, ContentAlignment.TopCenter);
                }
            }
        }
    }
}
