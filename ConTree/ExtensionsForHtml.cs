using System.Text;
using System.Collections.Generic;
using Kaos.SysIo;

namespace AppMain
{
    public static class EntensionsForHtml
    {
        public static StringBuilder AppendHtml (this StringBuilder sb, string source)
        {
            foreach (char ch in source)
                if (ch == '"')
                    sb.Append ("&quot;");
                else if (ch == '&')
                    sb.Append ("&amp;");
                else if (ch == '\'')
                    sb.Append ("&apos;");
                else if (ch == '<')
                    sb.Append ("&lt;");
                else if (ch == '>')
                    sb.Append ("&gt;");
                else
                    sb.Append (ch);

            return sb;
        }


        public static IEnumerable<string> TraverseForHtmlTree (this DirNode.Vector dv, string rootPath, bool showFiles=false)
        {
            int buttonId = 0;

            yield return ("<!DOCTYPE html>");
            yield return ("<html>");
            yield return ("<head>");

            var sb = new StringBuilder ("<title>");
            sb.AppendHtml (rootPath);
            sb.Append ("</title>");
            yield return sb.ToString();
            sb.Clear();

            yield return "<meta charset=\"UTF-8\">";
            yield return "<style>";
            yield return "  button.bn { border-width:1px; padding:0px 2px; font-family:monospace; font-size:xx-small; color: red; background-color:black; border-color:red; }";
            yield return "  div.s1 { display:block; }";
            yield return "  div.s2 { display:none; }";
            yield return "</style>";
            yield return "<script type=\"text/javascript\">";
            yield return "function tgl(btn,divName)";
            yield return "{";
            yield return "  var divId = document.getElementById(divName);";
            yield return "  if (divId.className == \"s1\")";
            yield return "  { divId.className = 's2'; btn.textContent = \"+\"; }";
            yield return "  else";
            yield return "  { divId.className = 's1'; btn.textContent = \"-\"; }";
            yield return "}";
            yield return "</script>";
            yield return "</head>";
            yield return "<body style='color:orange; background-color:black; font-family:monospace; font-size:medium; white-space:pre;'>";

            dv.Advance();
            sb.AppendHtml (dv.Items[0].Path);
            yield return sb.ToString();
            sb.Clear();

            for (bool hasSubdirsOrFiles = dv.PregetContents (showFiles);;)
            {
                if (hasSubdirsOrFiles && dv.Top.FileInfos != null && dv.Top.FileInfos.Count > 0)
                {
                    string indent = new StringBuilder().AppendIndent (dv, showFiles).ToString();
                    sb.Append (indent);
                    sb.AppendHtml (dv.Top.FileInfos[0].Name);
                    yield return sb.ToString();
                    sb.Clear();
                    sb.Append (indent);
                    for (int fi = 1; fi < dv.Top.FileInfos.Count; ++fi)
                    {
                        var fileName = dv.Top.FileInfos[fi].Name;
                        sb.AppendHtml (fileName);
                        yield return sb.ToString();
                        sb.Length = indent.Length;
                    }

                    yield return indent;
                    sb.Clear();
                }

                if (dv.Depth >= 1 && ! dv.HasSubdirs)
                {
                    if (dv.Top.FileInfos != null && dv.Top.FileInfos.Count > 0)
                        sb.Append ("</div>");
                    for (int dx = dv.Depth; dx > 1 && dv.Items[dx].IsLast; --dx)
                        sb.Append ("</div>");
                }

                if (! dv.Advance())
                    break;

                hasSubdirsOrFiles = dv.PregetContents (showFiles);
                if (! hasSubdirsOrFiles)
                {
                    sb.AppendIndent (dv, false);
                    sb.Append ("<button class='bn'> </button>");
                    sb.AppendHtml (dv.Top.Name);
                    yield return sb.ToString();
                    sb.Clear();
                }
                else
                {
                    ++buttonId;
                    sb.AppendIndent (dv, false);
                    sb.Append ("<button id='b");
                    sb.Append (buttonId);
                    sb.Append ("' class='bn' onclick=\"tgl(this,'d");
                    sb.Append (buttonId);
                    sb.Append ("')\">+</button>");
                    sb.AppendHtml (dv.Top.Name);
                    yield return sb.ToString();
                    sb.Clear();

                    sb.Append ("<div id='d");
                    sb.Append (buttonId);
                    sb.Append ("' class='s2'>");
                }
            }

            sb.Append ("</body>");
            yield return sb.ToString();
            yield return "</html>";
        }
    }
}
