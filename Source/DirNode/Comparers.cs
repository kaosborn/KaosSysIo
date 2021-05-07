using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Kaos.SysIo
{
    /// <summary>Define comparers for <see cref="DirectoryInfo"/> instances.</summary>
    public static class SafeNativeMethods
    {
        [DllImport ("shlwapi.dll", CharSet=CharSet.Unicode)]
        private static extern int StrCmpLogicalW (string s1, string s2);

        /// <summary>Encapsulate natural comparison operation for <see cref="DirectoryInfo"/> instances.</summary>
        public class NaturalCompareDirectoryInfo : Comparer<DirectoryInfo>
        {
            /// <summary>Define method for natural comparison of <see cref="DirectoryInfo"/> instances.</summary>
            public static readonly IComparer<DirectoryInfo> Comparer = new NaturalCompareDirectoryInfo();

            /// <summary>Perform a natural comparison of the supplied <see cref="DirectoryInfo"/> instances.</summary>
            /// <param name="d1">Instance for comparison.</param>
            /// <param name="d2">Instance for comparison.</param>
            /// <returns>A value indicating whether one <see cref="DirectoryInfo"/> is less than, equal to, or greater than the other.</returns>
            public override int Compare (DirectoryInfo d1, DirectoryInfo d2)
             => SafeNativeMethods.StrCmpLogicalW (d1.Name, d2.Name);
        }

        /// <summary>Encapsulate lexical comparison operation for <see cref="DirectoryInfo"/> instances.</summary>
        public class LexicalCompareDirectoryInfo : Comparer<DirectoryInfo>
        {
            /// <summary>Define method for lexical comparison of <see cref="DirectoryInfo"/> instances.</summary>
            public static readonly IComparer<DirectoryInfo> Comparer = new LexicalCompareDirectoryInfo();

            /// <summary>Perform a lexical comparison of the supplied <see cref="DirectoryInfo"/> instances.</summary>
            /// <param name="d1">Instance for comparison.</param>
            /// <param name="d2">Instance for comparison.</param>
            /// <returns>A value indicating whether one <see cref="DirectoryInfo"/> is less than, equal to, or greater than the other.</returns>
            public override int Compare (DirectoryInfo d1, DirectoryInfo d2)
             => String.CompareOrdinal (d1.Name, d2.Name);
        }

        /// <summary>Encapsulate natural comparison operation for <see cref="FileInfo"/> instances.</summary>
        public class NaturalCompareFileInfo : Comparer<FileInfo>
        {
            /// <summary>Define method for natural comparison of <see cref="FileInfo"/> instances.</summary>
            public static readonly IComparer<FileInfo> Comparer = new NaturalCompareFileInfo();

            /// <summary>Perform a lexical comparison of the supplied <see cref="FileInfo"/> instances.</summary>
            /// <param name="f1">Instance for comparison.</param>
            /// <param name="f2">Instance for comparison.</param>
            /// <returns>A value indicating whether one <see cref="FileInfo"/> is less than, equal to, or greater than the other.</returns>
            public override int Compare (FileInfo f1, FileInfo f2)
             => SafeNativeMethods.StrCmpLogicalW (f1.Name, f2.Name);
        }

        /// <summary>Encapsulate lexical comparison operation for <see cref="FileInfo"/> instances.</summary>
        public class LexicalCompareFileInfo : Comparer<FileInfo>
        {
            /// <summary>Define method for lexical comparison of <see cref="FileInfo"/> instances.</summary>
            public static readonly IComparer<FileInfo> Comparer = new LexicalCompareFileInfo();

            /// <summary>Perform a lexical comparison of the supplied <see cref="FileInfo"/> instances.</summary>
            /// <param name="f1">Instance for comparison.</param>
            /// <param name="f2">Instance for comparison.</param>
            /// <returns>A value indicating whether one <see cref="FileInfo"/> is less than, equal to, or greater than the other.</returns>
            public override int Compare (FileInfo f1, FileInfo f2)
             => String.CompareOrdinal (f1.Name, f2.Name);
        }
    }
}
