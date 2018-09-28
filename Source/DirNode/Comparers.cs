using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Kaos.SysIo
{
    public static class SafeNativeMethods
    {
        [DllImport ("shlwapi.dll", CharSet=CharSet.Unicode)]
        private static extern int StrCmpLogicalW (string s1, string s2);

        public class NaturalCompareDirectoryInfo : Comparer<DirectoryInfo>
        {
            public static readonly IComparer<DirectoryInfo> Comparer = new NaturalCompareDirectoryInfo();

            public override int Compare (DirectoryInfo d1, DirectoryInfo d2)
             => SafeNativeMethods.StrCmpLogicalW (d1.Name, d2.Name);
        }

        public class LexicalCompareDirectoryInfo : Comparer<DirectoryInfo>
        {
            public static readonly IComparer<DirectoryInfo> Comparer = new LexicalCompareDirectoryInfo();

            public override int Compare (DirectoryInfo d1, DirectoryInfo d2)
             => String.CompareOrdinal (d1.Name, d2.Name);
        }

        public class NaturalCompareFileInfo : Comparer<FileInfo>
        {
            public static readonly IComparer<FileInfo> Comparer = new NaturalCompareFileInfo();

            public override int Compare (FileInfo f1, FileInfo f2)
             => SafeNativeMethods.StrCmpLogicalW (f1.Name, f2.Name);
        }

        public class LexicalCompareFileInfo : Comparer<FileInfo>
        {
            public static readonly IComparer<FileInfo> Comparer = new LexicalCompareFileInfo();

            public override int Compare (FileInfo f1, FileInfo f2)
             => String.CompareOrdinal (f1.Name, f2.Name);
        }
    }
}
