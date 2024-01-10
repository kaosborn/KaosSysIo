// Program: DwBench01.exe
// Purpose: Exercise the DirWalker class using .NET Framework 4.8.

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
