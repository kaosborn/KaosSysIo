//
// Project: KaosSysIo
// File: ConTree.cs
//
// Purpose: Provide alternative to Windows tree.exe console program:
// - Adds option to sort naturally
// - Adds option to produce static web page output
// - Adds control over indention
//

using System;
using System.IO;
using Kaos.SysIo;

namespace AppMain
{
    enum TargetInterface { Terminal, Browser }

    class ConTree
    {
        static int Main (string[] args)
        {
            string err = null;
            string rootPath = null;
            string fileFilter = null;
            int tab = 4;
            TargetInterface target = TargetInterface.Terminal;
            DrawWith drawWith = DrawWith.Graphic;
            Ordering ordering = Ordering.None;

            for (int ix = 0; ix < args.Length; ++ix)
            {
                var arg = args[ix];
                if (arg == "/?")
                {
                    ShowUsage();
                    return 0;
                }
                else if (arg == "/A")
                    drawWith = DrawWith.Ascii;
                else if (arg == "/F")
                    fileFilter = "*";
                else if (arg == "/SL")
                    ordering = Ordering.Lexical;
                else if (arg == "/SN")
                    ordering = Ordering.Natural;
                else if (arg == "/W")
                    target = TargetInterface.Browser;
                else if (arg.StartsWith ("/"))
                {
                    bool isOk = Int32.TryParse (arg.Substring (1), out int tryTab);
                    if (isOk && tryTab > 0)
                        tab = tryTab;
                    else
                    { Console.WriteLine ("Invalid switch - " + args[ix]); return 1; }
                }
                else if (rootPath != null)
                { Console.WriteLine ("Too many parameters - " + args[ix]); return 2; }
                else
                    rootPath = arg;
            }

            if (rootPath == null)
                rootPath = ".";

            try
            {
                if (target == TargetInterface.Terminal)
                    foreach (string lx in DirNode.Vector.GenerateTextTree (rootPath, fileFilter, drawWith, ordering, tab))
                        Console.WriteLine (lx);
                else
                    foreach (string lx in DirVectorHtml.GenerateHtmlTree (rootPath, fileFilter, drawWith, ordering, tab))
                        Console.WriteLine (lx);
            }
            catch (IOException ex)
            { err = ex.Message.Trim(); }
            catch (UnauthorizedAccessException ex)
            { err = ex.Message.Trim(); }

            if (err != null)
            {
                Console.Error.WriteLine (err);
                return 1;
            }

            return 0;
        }


        static void ShowUsage()
        {
            string exe = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            Console.WriteLine ("Graphically displays the folder structure of a drive or path.");
            Console.WriteLine ();
            Console.WriteLine (exe + " [drive:][path] [/F] [/A] [/W] [/SL] [/SN] [/n]");
            Console.WriteLine ();
            Console.WriteLine ("   /F   Display the names of the files in each folder.");
            Console.WriteLine ("   /A   Use ASCII instead of extended characters.");
            Console.WriteLine ("   /W   Produce output suitable for a static HTML web page.");
            Console.WriteLine ("   /SL  Sort lexically (default).");
            Console.WriteLine ("   /SN  Sort naturally.");
            Console.WriteLine ("   /n   Indent by n where n is a number.");
        }
    }
}
