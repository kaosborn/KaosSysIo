﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Kaos.SysIo
{
    public enum Ordering { None, Lexical, Natural }
    public enum DrawWith { Ascii, Graphic }

    /// <summary>
    /// A single directory node with its index in its parent's subdirs.
    /// Root is always Index 0 of 1 FileInfo.
    /// </summary>
    [DebuggerDisplay(@"\{{Path}}")]
    public class DirNode
    {
        private DirectoryInfo[] dirInfos;
        public int Index { get; private set; }
        public ReadOnlyCollection<FileInfo> FileInfos { get; private set; }

        public string Path { get { return dirInfos[Index].FullName; } }
        public string Name { get { return dirInfos[Index].Name; } }
        public int DirCount { get { return dirInfos.Length; } }
        public bool IsLast { get { return Index >= DirCount - 1; } }


        private DirNode (DirectoryInfo[] dirInfos, int index)
        {
            this.dirInfos = dirInfos;
            this.Index = index;
        }


        public class Vector
        {
            private readonly IList<DirNode> items;
            public ReadOnlyCollection<DirNode> Items { get; private set; }

            private readonly IComparer<DirectoryInfo> dirComparer=null;
            private readonly IComparer<FileInfo> fileComparer=null;

            public string RootPath { get; private set; }
            public int Depth { get; private set; }
            public int TabSize { get; private set; }
            public char UpDown { get; private set; }
            public char LeftRight { get; private set; }
            public char UpRight { get; private set; }
            public char UpDownRight { get; private set; }

            public DirNode Top { get { return items[Depth]; } }
            public bool HasSubdirs { get { return Depth+1 < items.Count && items[Depth+1].dirInfos.Length > 0; } }
            public string TreeName { get { return items.Count == 1 ? RootPath : items[items.Count-1].Name; } }

            protected Vector (string rootPath, Ordering order=Ordering.None, DrawWith drawWith=DrawWith.Ascii, int tabSize=4)
            {
                this.items = new List<DirNode> { new DirNode (new DirectoryInfo[] { new DirectoryInfo (rootPath) }, -1) };
                this.Items = new ReadOnlyCollection<DirNode> (items);
                this.Depth = -1;
                this.TabSize = tabSize;

                if (drawWith == DrawWith.Graphic)
                { this.UpDown = '\u2502'; this.LeftRight = '\u2500'; this.UpRight = '\u2514'; this.UpDownRight = '\u251C'; }
                else
                { this.UpDown = '|'; this.LeftRight = '-'; this.UpRight = '\\'; this.UpDownRight = '+'; }

                if (order == Ordering.Lexical)
                { this.dirComparer = LexicalCompareDirectoryInfo.Comparer; this.fileComparer = LexicalCompareFileInfo.Comparer; }
                else if (order == Ordering.Natural)
                { this.dirComparer = NaturalCompareDirectoryInfo.Comparer; this.fileComparer = NaturalCompareFileInfo.Comparer; }
            }


            // On exit: returns true if node has subdirectories or files (if includeFiles)
            // Any subdirectories will be prefetched.
            protected bool PregetContents (bool includeFiles)
            {
                bool result;
                var top = items[items.Count - 1];
                if (top.Index < 0)
                {
                    result = top.dirInfos.Length > 0;
                    top = items[items.Count - 2];
                }
                else
                {
                    DirectoryInfo[] nextDirs = top.dirInfos[top.Index].GetDirectories();
                    if (dirComparer != null)
                        Array.Sort (nextDirs, dirComparer);
                    items.Add (new DirNode (nextDirs, -1));
                    result = nextDirs.Length > 0;
                }

                if (includeFiles)
                {
                    FileInfo[] fInfos = top.dirInfos[top.Index].GetFiles();
                    if (fileComparer != null)
                        Array.Sort (fInfos, fileComparer);
                    top.FileInfos = new ReadOnlyCollection<FileInfo> (fInfos);
                    result = result || top.FileInfos.Count > 0;
                }

                return result;
            }


            protected void Reset()
            {
                if (items.Count == 0)
                    items.Add (new DirNode (new DirectoryInfo[] { new DirectoryInfo (RootPath) }, -1));
                else if (items.Count > 1)
                    items.RemoveAt (1);
                items[0].Index = -1;
                Depth = -1;
            }


            protected bool Advance()
            {
                if (items.Count == 0)
                    return false;

                DirNode top = items[items.Count - 1];
                if (top.Index >= 0)
                {
                    DirectoryInfo[] subdirs = top.dirInfos[top.Index].GetDirectories();
                    if (subdirs.Length > 0)
                    {
                        if (dirComparer != null)
                            Array.Sort (subdirs, dirComparer);
                        Depth = items.Count;
                        items.Add (new DirNode (subdirs, 0));
                        return true;
                    }
                }

                for (;;)
                {
                    if (++top.Index < top.DirCount)
                    { Depth = items.Count - 1; return true; }

                    items.RemoveAt (items.Count - 1);
                    if (items.Count == 0)
                    { Depth = -1; return false; }

                    top = items[items.Count - 1];
                }
            }


            public static IEnumerable<string> EnumerateFiles (string rootPath, Ordering order=Ordering.None)
            {
                for (var dv = new DirNode.Vector (rootPath, order); dv.Advance();)
                {
                    dv.PregetContents (true);
                    foreach (var fInfo in dv.Top.FileInfos)
                        yield return fInfo.FullName;
                }
            }


            public static IEnumerable<string> EnumerateDirectories (string rootPath, Ordering order=Ordering.None)
            {
                for (var dv = new DirNode.Vector (rootPath, order); dv.Advance();)
                    yield return dv.Top.Path;
            }


            public static IEnumerable<DirectoryInfo> EnumerateDirectoriesForInfo (string rootPath, Ordering order=Ordering.None)
            {
                for (var dv = new DirNode.Vector (rootPath, order); dv.Advance();)
                    yield return dv.Top.dirInfos[dv.Top.Index];
            }


            public static IEnumerable<string> GenerateTextTree (string rootPath, bool showFiles=false, DrawWith viewChars=DrawWith.Graphic, Ordering order=Ordering.None, int tab=4)
            {
                var sb = new StringBuilder();
                for (var dv = new DirNode.Vector (rootPath, order); dv.Advance(); sb.Clear())
                {
                    sb.AppendIndent (dv, false);
                    sb.Append (dv.Top.Path);
                    yield return sb.ToString();

                    if (showFiles)
                    {
                        dv.PregetContents (true);
                        if (dv.Top.FileInfos.Count > 0)
                        {
                            sb.Clear();
                            sb.AppendIndent (dv, showFiles);
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
