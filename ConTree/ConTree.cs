using System;
using System.IO;
using Kaos.SysIo;

namespace AppMain
{
    enum ViewFormat { Text, Html }

    class ConTree
    {
        static int Main (string[] args)
        {
            string rootPath = null;
            bool showFiles = false;
            int tab = 4;
            bool is437 = true;
            bool isNaturalSort = false;
            ViewFormat viewFormat = ViewFormat.Text;
            string err = null;

            for (int ix = 0; ix < args.Length; ++ix)
            {
                var arg = args[ix];
                if (arg == "/?")
                {
                    ShowUsage();
                    return 0;
                }
                else if (arg == "/A")
                    is437 = false;
                else if (arg == "/F")
                    showFiles = true;
                else if (arg == "/N")
                    isNaturalSort = true;
                else if (arg == "/W")
                    viewFormat = ViewFormat.Html;
                else if (arg == "/2")
                    tab = 2;
                else if (arg.StartsWith ("/"))
                { Console.WriteLine ("Invalid switch - " + args[ix]); return 1; }
                else if (rootPath != null)
                { Console.WriteLine ("Too many parameters - " + args[ix]); return 2; }
                else
                    rootPath = arg;
            }

            if (rootPath == null)
                rootPath = ".";

            try
            {
                DirNode.Vector nodes = is437? DirNode.Vector.CreateGraphic (rootPath, tab, isNaturalSort)
                                            : DirNode.Vector.Create (rootPath, tab, isNaturalSort);

                if (viewFormat == ViewFormat.Text)
                    foreach (string lx in nodes.TraverseForTextTree (rootPath, showFiles))
                        Console.WriteLine (lx);
                else
                    foreach (string lx in nodes.TraverseForHtmlTree (rootPath, showFiles))
                        Console.WriteLine (lx);
            }
            catch (IOException ex)
            { err = ex.Message.Trim(); }
            catch (UnauthorizedAccessException ex)
            { err = ex.Message.Trim(); }

            if (err != null)
            {
                Console.WriteLine (err);
                return 1;
            }

            return 0;
        }


        static void ShowUsage()
        {
            string exe = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            Console.WriteLine ("Graphically displays the folder structure of a drive or path.");
            Console.WriteLine ();
            Console.WriteLine (exe + " [drive:][path] [/F] [/A] [/W] [/X] [/2]");
            Console.WriteLine ();
            Console.WriteLine ("   /F   Display the names of the files in each folder.");
            Console.WriteLine ("   /A   Use ASCII instead of extended characters.");
            Console.WriteLine ("   /W   Produce output suitable for a static HTML web page.");
            Console.WriteLine ("   /N   Use natural sort rather than lexical sort.");
            Console.WriteLine ("   /2   Indent by 2 instead of 4.");
        }
    }
}
