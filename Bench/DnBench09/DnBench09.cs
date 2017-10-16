// Program: DnBench09.dll
// Purpose: Demonstrate FxCore usage of the DirNode class.
//
// To execute from a command prompt run:
//   dotnet DnBench09.dll

using System;
using Kaos.SysIo;

namespace BenchApp
{
    class DnBench09
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
