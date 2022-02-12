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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace System
{
    public static class ExceptionExtensions
    {
        //static ExceptionExtensions()
        //{
        //    EventLogFile = new FileInfo(Path.Combine(ApplicationEnvironment.UserProductDataPath.FullName, "Event.log"));

        //    if (!EventLogFile.Exists)
        //    {
        //        using (BinaryWriter writer = BinaryWriterExtensions.Open(EventLogFile.Open(FileMode.Append, FileAccess.Write)))
        //        {
        //            writer.WriteString(ApplicationEnvironment.CompanyName);
        //            writer.WriteString(ApplicationEnvironment.ProductName);
        //            writer.WriteString(ApplicationEnvironment.FileVersion.FileVersion);
        //        }
        //    }
        //}

        //public static readonly FileInfo EventLogFile;

        public static DialogResult ShowMessage(this Exception e)
        {
            return ShowMessage(e, default(IWin32Window), default(string), MessageBoxIcon.Error, MessageBoxButtons.OK);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner)
        {
            return ShowMessage(e, owner, default(string), MessageBoxIcon.Error, MessageBoxButtons.OK);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner, string message)
        {
            return ShowMessage(e, owner, message, MessageBoxIcon.Error, MessageBoxButtons.OK);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner, MessageBoxIcon icon)
        {
            return ShowMessage(e, owner, default(string), icon, MessageBoxButtons.OK);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner, MessageBoxButtons buttons)
        {
            return ShowMessage(e, owner, default(string), MessageBoxIcon.Error, buttons);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner, MessageBoxIcon icon, MessageBoxButtons buttons)
        {
            return ShowMessage(e, owner, default(string), icon, buttons);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner, string message, MessageBoxIcon icon)
        {
            return ShowMessage(e, owner, message, icon, MessageBoxButtons.OK);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner, string message, MessageBoxButtons buttons)
        {
            return ShowMessage(e, owner, message, MessageBoxIcon.Error, MessageBoxButtons.OK);
        }

        public static DialogResult ShowMessage(this Exception e, IWin32Window owner, string message, MessageBoxIcon icon, MessageBoxButtons buttons)
        {
            if (e.HasValue())
            {
                string text;
                string errorType = (e = (e.GetBaseException() ?? e)).GetType().Name;

                message = message ?? e.Message;

                if (Localization.ContainsKey(errorType))
                {
                    message = Localization.Localize(errorType);
                }

                e.Print();
                owner = owner ?? Form.ActiveForm ?? Application.OpenForms.Cast<Form>().LastOrDefault();
                text = string.Join(Environment.NewLine, message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).RemoveNullOrEmptyElements().ToArray());

                return MessageBox.Show(owner, text, ApplicationEnvironment.ProductName, buttons, icon);
            }

            return DialogResult.None;
        }

        public static IEnumerable<Exception> ForEachException(this Exception e)
        {
            Exception current = e;

            while (current.HasValue())
            {
                yield return current;

                current = current.InnerException;
            }
        }

        //public static void WriteEntry(this Exception e)
        //{
        //    using (BinaryWriter writer = BinaryWriterExtensions.Open(EventLogFile.Open(FileMode.Append, FileAccess.Write)))
        //    {
        //        var errors = e.ForEachException().ToList();

        //        writer.Write(DateTime.Now);
        //        writer.Write(errors.Count);

        //        foreach (Exception current in errors)
        //        {
        //            writer.Write(current.ToString());
        //        }
        //    }
        //}
    }
}