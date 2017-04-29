﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Kaos.SysIo
{
    public static class Imports
    {
        [DllImport ("shlwapi.dll", CharSet=CharSet.Unicode)]
        public static extern int StrCmpLogicalW (string s1, string s2);
    }


    public class NaturalCompareDirectoryInfo : Comparer<DirectoryInfo>
    {
        public static readonly IComparer<DirectoryInfo> Comparer = new NaturalCompareDirectoryInfo();

        public override int Compare (DirectoryInfo d1, DirectoryInfo d2)
        { return Imports.StrCmpLogicalW (d1.Name, d2.Name); }
    }


    public class LexicalCompareDirectoryInfo : Comparer<DirectoryInfo>
    {
        public static readonly IComparer<DirectoryInfo> Comparer = new LexicalCompareDirectoryInfo();

        public override int Compare (DirectoryInfo d1, DirectoryInfo d2)
        { return String.CompareOrdinal (d1.Name, d2.Name); }
    }


    public class NaturalCompareFileInfo : Comparer<FileInfo>
    {
        public static readonly IComparer<FileInfo> Comparer = new NaturalCompareFileInfo();

        public override int Compare (FileInfo f1, FileInfo f2)
        { return Imports.StrCmpLogicalW (f1.Name, f2.Name); }
    }


    public class LexicalCompareFileInfo : Comparer<FileInfo>
    {
        public static readonly IComparer<FileInfo> Comparer = new LexicalCompareFileInfo();

        public override int Compare (FileInfo f1, FileInfo f2)
        { return String.CompareOrdinal (f1.Name, f2.Name); }
    }
}
