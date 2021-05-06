using System.Collections.Generic;
using System.Text;
using Kaos.SysIo;

namespace AppMain
{
    public class DirVectorHtml : DirNode.Vector
    {
        private DirVectorHtml (string rootPath, Ordering order=Ordering.None, DrawWith drawWith=DrawWith.Ascii, int tabSize=4) : base (rootPath, null, order, drawWith, tabSize)
        { }

        public static IEnumerable<string> GenerateHtmlTree (string rootPath, string fileFilter, DrawWith drawWith=DrawWith.Graphic, Ordering order=Ordering.None, int tab=4)
        {
            var dv = new DirVectorHtml (rootPath, order, drawWith, tab);
            int buttonId = 0;

            yield return "<!DOCTYPE html>";
            yield return "<html>";
            yield return "<head>";
            yield return $"<title>{rootPath}</title>";
            yield return "<meta charset=\"UTF-8\">";
            yield return "<style>";
            yield return "  button.bn { border-width:1px; padding:0px 2px; font-family:monospace; font-size:xx-small; color: red; background-color:black; border-color:red; }";
            yield return "  div.s1 { display:block; }";
            yield return "  div.s2 { display:none; }";
            yield return "</style>";
            yield return "<script type=\"text/javascript\">";
            yield return "function tgl(btn,divName) {";
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

            var sb = new StringBuilder();
            sb.AppendHtml (dv[0].Path);
            yield return sb.ToString();
            sb.Clear();

            for (bool hasSubdirsOrFiles = dv.PregetContents (fileFilter);;)
            {
                if (hasSubdirsOrFiles && dv.Top.FileInfos != null && dv.Top.FileInfos.Count > 0)
                {
                    string indent = new StringBuilder().AppendIndent (dv, fileFilter != null).ToString();
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
                    for (int dx = dv.Depth; dx > 1 && dv[dx].IsLast; --dx)
                        sb.Append ("</div>");
                }

                if (! dv.Advance())
                    break;

                hasSubdirsOrFiles = dv.PregetContents (fileFilter);
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
