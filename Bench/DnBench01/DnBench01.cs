// Program: DnBench01.exe
// Purpose: Exercise the DirNode class using .NET 8.0+.

using System;
using Kaos.SysIo;

namespace BenchApp
{
    class DnBench01
    {
        static void Main()
        {
            Console.WriteLine ("Directories:");
            foreach (string dirName in DirNode.Vector.EnumerateDirectories ("."))
                Console.WriteLine (dirName);

            Console.WriteLine ("\nFiles:");
            foreach (string fileName in DirNode.Vector.EnumerateFiles ("."))
                Console.WriteLine (fileName);
        }
    }
}
