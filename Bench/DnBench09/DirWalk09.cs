//
// Program: DirWalk09.cs
// Purpose: Demonstrate FxCore usage.
//
// Usage:
// • To execute from a command prompt run:
//     dotnet DirWalk09.dll

using System;
using Kaos.SysIo;

namespace ExampleApp
{
    class DirWalk09
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
