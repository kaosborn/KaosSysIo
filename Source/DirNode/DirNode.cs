﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Kaos.Collections;

namespace Kaos.SysIo
{
    /// <summary>Specifies sorting of output.</summary>
    public enum Ordering
    {
        /// <summary>Apply no sorting.</summary>
        None,
        /// <summary>Sort character by character.</summary>
        Lexical,
        /// <summary>Sort numbers as a single character.</summary>
        Natural
    }

    /// <summary>Specifies outline edge characters.</summary>
    public enum DrawWith
    {
        /// <summary>Limit output to ASCII only.</summary>
        Ascii,
        /// <summary>Use extended characters from code page 437.</summary>
        Graphic
    }

    /// <summary>Define a single directory node with its index in its parent's subdirectories.</summary>
    /// <remarks>
    /// *Root* is always Index 0 of 1 *FileInfo*.
    /// </remarks>
    [DebuggerDisplay(@"\{{Path}}")]
    public class DirNode
    {
        private readonly DirectoryInfo[] dirInfos;

        /// <summary>Offset into the <see cref="DirectoryInfo"/> collection.</summary>
        public int Index { get; private set; }

        /// <summary>Collection of files in this directory.</summary>
        public ReadOnlyCollection<FileInfo> FileInfos { get; private set; }

        /// <summary>Directory path to this node.</summary>
        public string Path => dirInfos[Index].FullName;

        /// <summary>Node name.</summary>
        public string Name => dirInfos[Index].Name;

        /// <summary>Returns *true* if at last valid position, else *false*.</summary>
        public bool IsLast => Index >= DirCount - 1;

        /// <summary>Gets number of valid positions.</summary>
        public int DirCount => dirInfos.Length;

        private DirNode (DirectoryInfo[] dirInfos, int index)
        {
            this.dirInfos = dirInfos;
            this.Index = index;
        }

        /// <summary>A collection of <see cref="DirNode"/> items.</summary>
        public class Vector
        {
            private readonly QueuedStack<DirNode> stack;
            private readonly IComparer<DirectoryInfo> dirComparer = null;
            private readonly IComparer<FileInfo> fileComparer = null;

            /// <summary>Gets the <see cref="DirNode"/> at the supplied location.</summary>
            /// <param name="index">Collection index.</param>
            /// <returns>Item at *index*.</returns>
            public DirNode this[int index] => stack[index];

            /// <summary>Gets the number of elements contained in the <see cref="DirNode"/>.</summary>
            public int Count => stack.TotalCount;

            /// <summary>Returns the number of directory levels.</summary>
            public int Depth => stack.Count - 1;

            /// <summary>Gets the directory search pattern.</summary>
            protected string DirFilter { get; private set; }

            /// <summary>Topmost directory.</summary>
            public string RootPath { get; private set; }

            /// <summary>Indentation per level.</summary>
            public int TabSize { get; private set; }

            /// <summary>Vertical connector.</summary>
            public char UpDown { get; private set; }

            /// <summary>Horizontal connector.</summary>
            public char LeftRight { get; private set; }

            /// <summary>Bottom-left corner connector.</summary>
            public char UpRight { get; private set; }

            /// <summary>Left-T connector.</summary>
            public char UpDownRight { get; private set; }

            /// <summary>Gets the element at the top of the stack without removing it.</summary>
            public DirNode Top => stack.Peek();

            /// <summary>Returns *true* if move levels, else *false*.</summary>
            public bool HasSubdirs => Depth + 1 < stack.TotalCount && stack[Depth + 1].dirInfos.Length > 0;

            /// <summary>Initializes a new instance of the <see cref="DirNode.Vector"/> class.</summary>
            /// <param name="rootPath">Top directory.</param>
            /// <param name="dirFilter">Search pattern.</param>
            /// <param name="order">Output sorting.</param>
            /// <param name="drawWith">Outline characters.</param>
            /// <param name="tabSize">Number of characters to indent per level.</param>
            protected Vector (string rootPath, string dirFilter=null, Ordering order=Ordering.None, DrawWith drawWith=DrawWith.Ascii, int tabSize=4)
            {
                this.stack = new QueuedStack<DirNode>();
                this.stack.Enqueue (new DirNode (new DirectoryInfo[] { new DirectoryInfo (rootPath) }, -1));

                this.TabSize = tabSize;
                this.DirFilter = dirFilter?? "*";

                if (drawWith == DrawWith.Graphic)
                { UpDown = '\u2502'; LeftRight = '\u2500'; UpRight = '\u2514'; UpDownRight = '\u251C'; }
                else
                { UpDown = '|';      LeftRight = '-';      UpRight = '\\';     UpDownRight = '+'; }

                if (order == Ordering.Lexical)
                {
                    this.dirComparer = SafeNativeMethods.LexicalCompareDirectoryInfo.Comparer;
                    this.fileComparer = SafeNativeMethods.LexicalCompareFileInfo.Comparer;
                }
                else if (order == Ordering.Natural)
                {
                    this.dirComparer = SafeNativeMethods.NaturalCompareDirectoryInfo.Comparer;
                    this.fileComparer = SafeNativeMethods.NaturalCompareFileInfo.Comparer;
                }
            }

            /// <summary>Prefetch next level of subdirectories and files.</summary>
            /// <param name="fileFilter">Search pattern or *null* for all.</param>
            /// <returns>Returns *true* if more subdirectories or files available, else *false*.</returns>
            protected bool PregetContents (string fileFilter)
            {
                bool result;
                var top = stack.Peek();
                if (top.Index < 0)
                {
                    result = top.dirInfos.Length > 0;
                    top = stack.Head();
                }
                else
                {
                    DirectoryInfo[] nextDirs = top.dirInfos[top.Index].GetDirectories (DirFilter);
                    if (dirComparer != null)
                        Array.Sort (nextDirs, dirComparer);
                    stack.Enqueue (new DirNode (nextDirs, -1));
                    result = nextDirs.Length > 0;
                }

                if (fileFilter != null)
                {
                    FileInfo[] fInfos = top.dirInfos[top.Index].GetFiles (fileFilter);
                    if (fileComparer != null)
                        Array.Sort (fInfos, fileComparer);
                    top.FileInfos = new ReadOnlyCollection<FileInfo> (fInfos);
                    result = result || top.FileInfos.Count > 0;
                }

                return result;
            }


            /// <summary>Initialize the iterator.</summary>
            protected void Reset()
            {
                stack.Clear();
                stack.Push (new DirNode (new DirectoryInfo[] { new DirectoryInfo (RootPath) }, -1));
            }

            /// <summary>Advance the iterator one position.</summary>
            /// <returns>Returns *true* if not past end, else *false*.</returns>
            protected bool Advance()
            {
                if (stack.TotalCount == 0)
                    return false;

                DirNode top = stack[stack.TotalCount - 1];
                if (top.Index >= 0)
                {
                    DirectoryInfo[] subdirs = top.dirInfos[top.Index].GetDirectories (DirFilter);
                    if (subdirs.Length > 0)
                    {
                        if (dirComparer != null)
                            Array.Sort (subdirs, dirComparer);
                        stack.Push (new DirNode (subdirs, 0));
                        return true;
                    }
                }

                for (;;)
                {
                    if (++top.Index < top.DirCount)
                    {
                        if (stack.Count < stack.TotalCount)
                            stack.Push();
                        return true;
                    }

                    stack.RemoveAt (stack.TotalCount - 1);
                    if (stack.TotalCount == 0)
                        return false;

                    top = stack[stack.TotalCount-1];
                }
            }

            /// <summary>Yield all subdirectories of the supplied path.</summary>
            /// <param name="rootPath">Topmost directory.</param>
            /// <param name="fileFilter">Search pattern or *null* for all.</param>
            /// <param name="order">Output sorting.</param>
            /// <returns>All subdirectories of the supplied path as text.</returns>
            public static IEnumerable<string> EnumerateFiles (string rootPath, string fileFilter=null, Ordering order=Ordering.None)
            {
                if (fileFilter == null)
                    fileFilter = "*";

                for (var dv = new DirNode.Vector (rootPath, null, order); dv.Advance(); )
                {
                    dv.PregetContents (fileFilter);
                    foreach (var fInfo in dv.Top.FileInfos)
                        yield return fInfo.FullName;
                }
            }

            /// <summary>Yield all subdirectories of the supplied path.</summary>
            /// <param name="rootPath">Topmost directory.</param>
            /// <param name="filter">Search pattern or *null* for all.</param>
            /// <param name="order">Output sorting.</param>
            /// <returns>All subdirectories of the supplied path as text.</returns>
            public static IEnumerable<string> EnumerateDirectories (string rootPath, string filter=null, Ordering order=Ordering.None)
            {
                for (var dv = new DirNode.Vector (rootPath, filter, order); dv.Advance(); )
                    yield return dv.Top.Path;
            }

            /// <summary>Yield all subdirectories of the supplied path.</summary>
            /// <param name="rootPath">Topmost directory.</param>
            /// <param name="filter">Search pattern or *null* for all.</param>
            /// <param name="order">Output sorting.</param>
            /// <returns>All subdirectories of the supplied path as <see cref="DirectoryInfo"/> instances.</returns>
            public static IEnumerable<DirectoryInfo> EnumerateDirectoriesForInfo (string rootPath, string filter=null, Ordering order=Ordering.None)
            {
                for (var dv = new DirNode.Vector (rootPath, filter, order); dv.Advance(); )
                    yield return dv.Top.dirInfos[dv.Top.Index];
            }

            /// <summary>Yield all subdirectories of the supplied path as a text outline.</summary>
            /// <param name="rootPath">Topmost directory.</param>
            /// <param name="fileFilter">Search pattern or *null* for all.</param>
            /// <param name="drawWith">Outline characters.</param>
            /// <param name="order">Output sorting.</param>
            /// <param name="tab">Number of characters to indent per level.</param>
            /// <returns>All subdirectories of the supplied path as a text outline.</returns>
            public static IEnumerable<string> GenerateTextTree (string rootPath, string fileFilter, DrawWith drawWith=DrawWith.Graphic, Ordering order=Ordering.None, int tab=4)
            {
                var sb = new StringBuilder();
                for (var dv = new DirNode.Vector (rootPath, null, order, drawWith, tab); dv.Advance(); sb.Length = 0)
                {
                    sb.AppendIndent (dv, false);
                    sb.Append (dv.Depth == 0 ? dv.Top.Path : dv.Top.Name);
                    yield return sb.ToString();

                    if (fileFilter != null)
                    {
                        dv.PregetContents (fileFilter);
                        if (dv.Top.FileInfos.Count > 0)
                        {
                            sb.Length = 0;
                            sb.AppendIndent (dv, true);
                            int indentLength = sb.Length;
                            foreach (var fInfo in dv.Top.FileInfos)
                            {
                                sb.Append (fInfo.Name);
                                yield return sb.ToString();
                                sb.Length = indentLength;
                            }
                            yield return sb.ToString();
                        }
                    }
                }
            }
        }
    }
}
