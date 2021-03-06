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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace System.Configuration
{
    public class ApplicationConfiguration : XmlDocumentBase
    {
        static ApplicationConfiguration()
        {
            Current = new ApplicationConfiguration();
            Current.Load();
        }

        ApplicationConfiguration()
        {
            File = new FileInfo(Path.Combine(ApplicationEnvironment.UserProductDataPath.FullName, "app.config"));
            LoadXml("<Configuration></Configuration>");
        }

        protected FileInfo File;

        protected ApplicationAppearanceSettings AppearanceSettings;
        protected ApplicationLanguageSettings LanguageSettings;
        protected ApplicationSecuritySettings SecuritySettings;
        protected ApplicationSnapSettings SnapSettings;
        protected ApplicationAligmentGuidesSettings AligmentGuidesSettings;
        protected ApplicationOptions Settings;
        protected ComponentSettingCollection Components;
        protected RecentPathCollection Recent;

        public static ApplicationConfiguration Current
        {
            get;
            private set;
        }

        public ApplicationAppearanceSettings Appearance
        {
            get { return AppearanceSettings ?? (AppearanceSettings = CreateInstance<ApplicationAppearanceSettings>()); }
        }

        public ApplicationLanguageSettings Language
        {
            get { return LanguageSettings ?? (LanguageSettings = CreateInstance<ApplicationLanguageSettings>()); }
        }

        public ApplicationSecuritySettings Security
        {
            get { return SecuritySettings ?? (SecuritySettings = CreateInstance<ApplicationSecuritySettings>()); }
        }

        public ApplicationSnapSettings Snap
        {
            get { return SnapSettings ?? (SnapSettings = CreateInstance<ApplicationSnapSettings>()); }
        }

        public ApplicationAligmentGuidesSettings AlignmentGuides
        {
            get { return AligmentGuidesSettings ?? (AligmentGuidesSettings = CreateInstance<ApplicationAligmentGuidesSettings>()); }
        }

        public ApplicationOptions Options
        {
            get { return Settings ?? (Settings = CreateInstance<ApplicationOptions>()); }
        }

        public ComponentSettingCollection ComponentSettings
        {
            get { return Components ?? (Components = CreateInstance<ComponentSettingCollection>()); }
        }

        public RecentPathCollection RecentList
        {
            get { return Recent ?? (Recent = CreateInstance<RecentPathCollection>()); }
        }

        protected TElement CreateInstance<TElement>() where TElement : ApplicationConfigurationElement
        {
            string name = typeof(TElement).GetAttribute<DisplayNameAttribute>().DisplayName;
            return (TElement)(DocumentElement[name] ?? DocumentElement.AppendChild(CreateElement(name)));
        }

        public void Load()
        {
            if (File.Exists)
            {
                if (Thread.CurrentThread.IsBackground)
                {
                    Thread.Sleep(50);
                }

                Load(File.FullName);
            }
        }

        public void Save()
        {
            Save(File.FullName);
        }

        public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
        {
            switch (localName)
            {
                case "Appearance":
                    return new ApplicationAppearanceSettings(prefix, localName, namespaceURI, this);

                case "Language":
                    return new ApplicationLanguageSettings(prefix, localName, namespaceURI, this);

                case "Security":
                    return new ApplicationSecuritySettings(prefix, localName, namespaceURI, this);

                case "Snap":
                    return new ApplicationSnapSettings(prefix, localName, namespaceURI, this);

                case "AlignmentGuides":
                    return new ApplicationAligmentGuidesSettings(prefix, localName, namespaceURI, this);

                case "Options":
                    return new ApplicationOptions(prefix, localName, namespaceURI, this);

                case "Components":
                    return new ComponentSettingCollection(prefix, localName, namespaceURI, this);

                case "Component":
                    return new ComponentSetting(prefix, localName, namespaceURI, this);

                case "Recents":
                    return new RecentPathCollection(prefix, localName, namespaceURI, this);

                case "Properties":
                    return new ConfigurationNameValueCollection(prefix, localName, namespaceURI, this);
            }

            return base.CreateElement(prefix, localName, namespaceURI);
        }

        public void ValidateProcess()
        {
            if (DeveloperEnvironment.RuntimeMode)
            {
                Process current = Process.GetCurrentProcess();
                Assembly entryAssembly = Assembly.GetEntryAssembly();

                if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(entryAssembly.Location)).Count(e => e.Id != current.Id) > 0)
                {
                    current.Kill();
                }
            }
        }
    }
}
