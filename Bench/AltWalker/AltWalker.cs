//
// Program: AltWalker.cs
// Purpose: Demonstrate usage of the LiteDirWalker class.
//

using System;
using System.IO;
using Kaos.SysIo;

namespace BenchApp
{
    class LiteWalkBench01
    {
        static void Main()
        {
            foreach (var dx in new DirWalker (".."))
            {
                Console.WriteLine (dx);
                foreach (var fx in Directory.EnumerateFiles (dx))
                    Console.WriteLine ("  " + fx);
            }
        }
    }
}
