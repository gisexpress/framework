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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace System
{
    public static class IOExtensions
    {
        public static byte[] ReadAllBytes(this FileInfo file)
        {
            return File.ReadAllBytes(file.FullName);
        }

        public static bool WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            if (bytes.HasValue() && bytes.Length > 0)
            {
                File.WriteAllBytes(file.FullName, bytes);
                return true;
            }

            return false;
        }

        public static FileInfo ChangeExtension(this FileInfo value, string extension)
        {
            return new FileInfo(Path.ChangeExtension(value.FullName, extension));
        }

        public static string GetFileNameWithoutExtension(this FileInfo value)
        {
            return Path.GetFileNameWithoutExtension(value.Name);
        }

        public static bool IsSystemOrHiddenFile(this FileSystemInfo value)
        {
            return value.Attributes.HasFlag(FileAttributes.System) || value.Attributes.HasFlag(FileAttributes.Hidden);
        }

        public static Bitmap LoadImage(this FileInfo file)
        {
            if (file.HasValue() && file.Exists)
            {
                var image = BitmapExtensions.NewImage(Image.FromFile(file.FullName));

                try
                {
                    var bgColor = image.GetPixel(1, 1);

                    if (bgColor != Color.Transparent)
                    {
                        image.MakeTransparent(bgColor);
                    }
                }
                catch
                {
                }

                return image;
            }

            return default(Bitmap);
        }

        public static Version ToVersion(this FileVersionInfo fileVersion)
        {
            return new Version(fileVersion.FileMajorPart, fileVersion.FileMinorPart, fileVersion.FileBuildPart);
        }

        public static AssemblyName GetAssemblyName(this FileInfo file)
        {
            AssemblyName assemblyRef;

            if (TryGetAssemblyName(file, out assemblyRef))
            {
                return assemblyRef;
            }

            return default(AssemblyName);
        }

        public static bool TryGetAssemblyName(this FileInfo file, out AssemblyName assemblyRef)
        {
            if (file.IsAssemblyFile())
            {
                try
                {
                    assemblyRef = AssemblyName.GetAssemblyName(file.FullName);
                }
                catch
                {
                    assemblyRef = null;
                }

                return assemblyRef.HasValue();
            }

            assemblyRef = null;
            return false;
        }

        public static bool IsAssemblyFile(this FileInfo file)
        {
            using (Stream fs = file.OpenRead())
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    long n;

                    if (fs.Length > 0x40)
                    {
                        fs.Position = 0x3C;
                        n = reader.ReadUInt32();

                        if (n > 0 && n < fs.Length)
                        {
                            n = (fs.Position = n) + 0xE8;

                            if (n < fs.Length)
                            {
                                fs.Position = n;
                                return reader.ReadUInt32() != 0;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo directory, string extension)
        {
            return GetAllFiles(directory, extension, SearchOption.AllDirectories);
        }

        public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo directory, string extension, SearchOption searchOption)
        {
            if (directory.Exists)
            {
                extension = string.Concat('.', extension.TrimStart('.'));

                foreach (FileInfo file in directory.GetFiles("*.*", searchOption))
                {
                    if (file.Extension.EqualsIgnoreCase(extension))
                    {
                        yield return file;
                    }
                }
            }
        }

        public static bool IsEquivalent(this DirectoryInfo directory, DirectoryInfo value)
        {
            return directory.FullName.TrimEnd(Path.DirectorySeparatorChar).EqualsIgnoreCase(value.FullName.TrimEnd(Path.DirectorySeparatorChar));
        }

        public static bool ExistsAsync(this DirectoryInfo directory)
        {
            var r = new Task<bool>(() => { return directory.Exists; });

            r.Start();

            if (r.Wait(100))
            {
                return r.Result;
            }

            return false;
        }

        public class TempFile : IDisposable
        {
            public TempFile()
            {
                Name = Path.GetTempFileName();
            }

            public TempFile(string extension)
            {
                Name = Path.ChangeExtension(Path.GetTempFileName(), extension);
            }

            public readonly string Name;

            public void Dispose()
            {
                if (File.Exists(Name))
                {
                    File.Delete(Name);
                }
            }
        }
    }
}