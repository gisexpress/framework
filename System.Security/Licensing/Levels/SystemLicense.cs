//////////////////////////////////////////////////////////////////////////////////////////////////
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

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Accounts;

namespace System.Security
{
    [Guid("73A954C1-2394-4F56-9AEE-8113D8E7E3FA")]
    internal class SystemLicense : License
    {
        static SystemLicense()
        {
            //#if ReleaseMode
            //            if (4 > (int)Environment.OSVersion.Platform && DeveloperEnvironment.MonoFramework)
            //            {
            //                Console.WriteLine("Should be used Microsoft .NET Framework for Windows OS");
            //                Kill();
            //            }
            //#endif
        }

        protected SystemLicense()
        {
            LicenseFile = AccountManagerInternal.LicenseFile.Current;

            if (GetType().IsEquivalent(typeof(SystemLicense)))
            {
                AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
            }
        }

        protected bool DomainValidated;
        protected readonly AccountManagerInternal.LicenseFile LicenseFile;

        public bool IsValid()
        {
            if (OnValidate())
            {
                return true;
            }

            return false;
        }

        public override string LicenseKey
        {
            get { return GetType().GUID.ToString(); }
        }

        protected virtual bool OnValidate()
        {
            if (DomainValidated == false)
            {
                foreach (Assembly e in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (e.IsSystemAssembly())
                    {
                        continue;
                    }

                    Kill();
                }
            }

            return DomainValidated = true;
        }

        protected virtual void OnAssemblyLoad(object sender, AssemblyLoadEventArgs e)
        {
            if (e.LoadedAssembly.IsSystemAssembly())
            {
                return;
            }

            Kill();
        }

        internal static void Kill()
        {
//#if DEBUG
//            return;
//#endif

//            Process.GetCurrentProcess().Kill();
        }

        public override string ToString()
        {
            return Localization.Localize(LicenseKey);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
