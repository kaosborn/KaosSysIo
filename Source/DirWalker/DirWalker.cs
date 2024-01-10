// Project: KaosSysIo
// File:    DirWalker.cs
// Purpose: Define lite alternative to the DirNode class.
//          This is a minimal directory traverser without many features present in DirNode.Vector.

using System.Collections.Generic;
using System.IO;

namespace Kaos.SysIo
{
    /// <summary>Provide iterator for directory traversal.</summary>
    public class DirWalker : IEnumerable<string>
    {
        private readonly string[] dirs;
        private int index;

        /// <summary>Generate names of subdirecties under the specified directory.</summary>
        /// <param name="root">The path for which subdirectory names are yielded.</param>
        public DirWalker (string root)
         => this.dirs = new string[] { root };

        private DirWalker (string[] dirs)
         => this.dirs = dirs;

        /// <summary>Returns an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator that can be used to iterate thru the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
         => GetEnumerator();

        /// <summary>Returns an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator that can be used to iterate thru the collection.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            var stack = new Stack<DirWalker>();
            for (var node = this;;)
            {
                string dirName = node.dirs[node.index];
                yield return dirName;

                if (Directory.Exists (dirName))
                {
                    string[] subdirs = Directory.GetDirectories (dirName);
                    if (subdirs.Length > 0)
                    {
                        stack.Push (node);
                        node = new DirWalker (subdirs);
                        continue;
                    }
                }

                for (; ++node.index >= node.dirs.Length; node = stack.Pop())
                    if (stack.Count == 0)
                        yield break;
            }
        }
    }
}
