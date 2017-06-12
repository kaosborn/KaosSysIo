//
// Project: KaosSysIo
// File:    LiteDirWalker.cs
// Purpose: Define lite alternative to the DirNode class.
//          

using System.Collections.Generic;
using System.IO;

namespace AltSysIo
{
    /// <summary>Provide iterator for directory traversal.</summary>
    public class LiteDirWalker : IEnumerable<string>
    {
        private string[] dirs;
        private int index;

        /// <summary>Generate names of subdirecties under the specified directory.</summary>
        /// <param name="root">The path for which subdirectory names are yielded.</param>
        public LiteDirWalker (string root)
        { this.dirs = new string[] { root }; }

        private LiteDirWalker (string[] dirs)
        { this.dirs = dirs; }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public IEnumerator<string> GetEnumerator()
        {
            var stack = new Stack<LiteDirWalker>();
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
                        node = new LiteDirWalker (subdirs);
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
