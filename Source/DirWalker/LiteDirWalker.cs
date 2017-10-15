//
// Project: KaosSysIo
// File:    DirWalker.cs
// Purpose: Define lite alternative to the DirNode class.
//          This is a minimal solution without the features present in DirNode.Vector.
//

using System.Collections.Generic;
using System.IO;

namespace Kaos.SysIo
{
    /// <summary>Provide iterator for directory traversal.</summary>
    public class DirWalker : IEnumerable<string>
    {
        private string[] dirs;
        private int index;

        /// <summary>Generate names of subdirecties under the specified directory.</summary>
        /// <param name="root">The path for which subdirectory names are yielded.</param>
        public DirWalker (string root)
        { this.dirs = new string[] { root }; }

        private DirWalker (string[] dirs)
        { this.dirs = dirs; }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

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
