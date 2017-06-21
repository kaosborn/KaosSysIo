using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Kaos.Collections;

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
            private readonly QueuedStack<DirNode> stack;
            private readonly IComparer<DirectoryInfo> dirComparer=null;
            private readonly IComparer<FileInfo> fileComparer=null;

            public DirNode this[int index] { get { return stack[index]; } }
            public int Count { get { return stack.Count; } }
            public int Depth { get { return stack.Height-1; } }
            protected string DirFilter { get; private set; }
            public string RootPath { get; private set; }
            public int TabSize { get; private set; }

            public char UpDown { get; private set; }
            public char LeftRight { get; private set; }
            public char UpRight { get; private set; }
            public char UpDownRight { get; private set; }

            public DirNode Top { get { return stack.Peek(); } }
            public bool HasSubdirs { get { return Depth+1 < stack.Count && stack[Depth+1].dirInfos.Length > 0; } }


            protected Vector (string rootPath, string dirFilter=null, Ordering order=Ordering.None, DrawWith drawWith=DrawWith.Ascii, int tabSize=4)
            {
                this.stack = new QueuedStack<DirNode>();
                this.stack.Enqueue (new DirNode (new DirectoryInfo[] { new DirectoryInfo (rootPath) }, -1));

                this.TabSize = tabSize;
                this.DirFilter = dirFilter?? "*";

                if (drawWith == DrawWith.Graphic)
                { this.UpDown = '\u2502'; this.LeftRight = '\u2500'; this.UpRight = '\u2514'; this.UpDownRight = '\u251C'; }
                else
                { this.UpDown = '|'; this.LeftRight = '-'; this.UpRight = '\\'; this.UpDownRight = '+'; }

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


            // On exit: returns true if node has subdirectories or files.
            // Any subdirectories will be prefetched.
            protected bool PregetContents (string fileFilter)
            {
                bool result;
                var top = stack.Peek();
                if (top.Index < 0)
                {
                    result = top.dirInfos.Length > 0;
                    top = stack.Next();
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


            protected void Reset()
            {
                stack.Clear();
                stack.Push (new DirNode (new DirectoryInfo[] { new DirectoryInfo (RootPath) }, -1));
            }


            protected bool Advance()
            {
                if (stack.Count == 0)
                    return false;

                DirNode top = stack[stack.Count - 1];
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
                        if (stack.Height < stack.Count)
                            stack.Push();
                        return true;
                    }

                    stack.RemoveAt (stack.Count - 1);
                    if (stack.Count == 0)
                        return false;

                    top = stack[stack.Count-1];
                }
            }


            public static IEnumerable<string> EnumerateFiles (string rootPath, string fileFilter=null, Ordering order=Ordering.None)
            {
                if (fileFilter == null)
                    fileFilter = "*";

                for (var dv = new DirNode.Vector (rootPath, null, order); dv.Advance();)
                {
                    dv.PregetContents (fileFilter);
                    foreach (var fInfo in dv.Top.FileInfos)
                        yield return fInfo.FullName;
                }
            }


            public static IEnumerable<string> EnumerateDirectories (string rootPath, string filter=null, Ordering order=Ordering.None)
            {
                for (var dv = new DirNode.Vector (rootPath, filter, order); dv.Advance();)
                    yield return dv.Top.Path;
            }


            public static IEnumerable<DirectoryInfo> EnumerateDirectoriesForInfo (string rootPath, string filter=null, Ordering order=Ordering.None)
            {
                for (var dv = new DirNode.Vector (rootPath, filter, order); dv.Advance();)
                    yield return dv.Top.dirInfos[dv.Top.Index];
            }


            public static IEnumerable<string> GenerateTextTree (string rootPath, string fileFilter, DrawWith drawWith=DrawWith.Graphic, Ordering order=Ordering.None, int tab=4)
            {
                var sb = new StringBuilder();
                for (var dv = new DirNode.Vector (rootPath, null, order, drawWith, tab); dv.Advance(); sb.Clear())
                {
                    sb.AppendIndent (dv, false);
                    sb.Append (dv.Top.Path);
                    yield return sb.ToString();

                    if (fileFilter != null)
                    {
                        dv.PregetContents (fileFilter);
                        if (dv.Top.FileInfos.Count > 0)
                        {
                            sb.Clear();
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
