// Program: DwBench01.exe
// Purpose: Exercise the DirWalker class.

using System;
using System.IO;
using Kaos.SysIo;

namespace BenchApp
{
    class DwBench01
    {
        static void Main()
        {
            foreach (string dx in new DirWalker (".."))
            {
                Console.WriteLine (dx);
                foreach (string fx in Directory.EnumerateFiles (dx))
                    Console.WriteLine ($"  {fx}");
            }
        }
    }
}
