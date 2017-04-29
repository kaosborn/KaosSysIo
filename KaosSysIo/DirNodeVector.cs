using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Kaos.SysIo
{
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

            public string RootPath { get; private set; }
            public int Depth { get; private set; }

            public int TabSize { get; private set; }
            public bool IsNaturalSort { get; private set; }
            public char UpDown { get; private set; }
            public char LeftRight { get; private set; }
            public char UpRight { get; private set; }
            public char UpDownRight { get; private set; }

            public DirNode Top { get { return items[Depth]; } }
            public bool HasSubdirs { get { return Depth+1 < items.Count && items[Depth+1].dirInfos.Length > 0; } }
            public string TreeName { get { return items.Count == 1 ? RootPath : items[items.Count-1].Name; } }

            private Vector()
            {
                this.items = new List<DirNode>();
                this.Items = new ReadOnlyCollection<DirNode> (items);
                Depth = -1;
            }

            private Vector (string rootPath, int tabSize, bool isNaturalSort, char upDown, char leftRight, char upRight, char upDownRight) : this()
            {
                this.items.Add (new DirNode (new DirectoryInfo[] { new DirectoryInfo (rootPath) }, -1));

                this.TabSize = tabSize;
                this.IsNaturalSort = isNaturalSort;
                this.UpDown = upDown;
                this.LeftRight = leftRight;
                this.UpRight = upRight;
                this.UpDownRight = upDownRight;
            }

            public static Vector Create (string rootPath, int tabSize=4, bool isNaturalSort=true)
            { return new Vector (rootPath, tabSize, isNaturalSort, '|', '-', '\\', '+'); }

            public static Vector CreateGraphic (string rootPath, int tabSize=4, bool isNaturalSort=true)
            { return new Vector (rootPath, tabSize, isNaturalSort, '\u2502', '\u2500', '\u2514', '\u251C'); }


            // On exit: returns true if node has subdirectories or files (if includeFiles)
            // Any subdirectories will be prefetched.
            public bool PregetContents (bool includeFiles)
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
                    Array.Sort (nextDirs, IsNaturalSort? NaturalCompareDirectoryInfo.Comparer : LexicalCompareDirectoryInfo.Comparer);
                    items.Add (new DirNode (nextDirs, -1));
                    result = nextDirs.Length > 0;
                }

                if (includeFiles)
                {
                    FileInfo[] fInfos = top.dirInfos[top.Index].GetFiles();
                    IComparer<FileInfo> cfi = IsNaturalSort? NaturalCompareFileInfo.Comparer : LexicalCompareFileInfo.Comparer;
                    Array.Sort (fInfos, cfi);
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


            public bool Advance()
            {
                if (items.Count == 0)
                    return false;

                DirNode top = items[items.Count - 1];
                if (top.Index >= 0)
                {
                    DirectoryInfo[] subdirs = top.dirInfos[top.Index].GetDirectories();
                    if (subdirs.Length > 0)
                    {
                        Array.Sort (subdirs, IsNaturalSort? NaturalCompareDirectoryInfo.Comparer : LexicalCompareDirectoryInfo.Comparer);
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


            public static IEnumerable<string> TraverseForPath (string rootPath)
            {
                var nodes = DirNode.Vector.Create (rootPath);
                while (nodes.Advance())
                    yield return nodes.Top.Path;
            }


            public static IEnumerable<DirectoryInfo> TraverseForInfo (string rootPath)
            {
                var nodes = DirNode.Vector.Create (rootPath);
                while (nodes.Advance())
                    yield return nodes.Top.dirInfos[nodes.Top.Index];
            }


            public IEnumerable<string> TraverseForTextTree (string rootPath, bool showFiles=false)
            {
                for (var sb = new StringBuilder(); Advance(); sb.Clear())
                {
                    sb.AppendIndent (this, false);
                    sb.Append (Top.Name);
                    yield return sb.ToString();

                    if (showFiles)
                    {
                        PregetContents (true);
                        if (Top.FileInfos.Count > 0)
                        {
                            sb.Clear();
                            sb.AppendIndent (this, showFiles);
                            int indentLength = sb.Length;
                            foreach (var fInfo in Top.FileInfos)
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
