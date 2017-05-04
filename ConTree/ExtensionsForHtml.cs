using System.Text;

namespace AppMain
{
    public static class EntensionsForHtml
    {
        public static StringBuilder AppendHtml (this StringBuilder sb, string source)
        {
            foreach (char ch in source)
            {
                if (ch <= '>')
                    if (ch == '>')
                        sb.Append ("&gt;");
                    else if (ch == '<')
                        sb.Append ("&lt;");
                    else if (ch == '\'')
                        sb.Append ("&apos;");
                    else if (ch == '&')
                        sb.Append ("&amp;");
                    else if (ch == '"')
                        sb.Append ("&quot;");

                sb.Append (ch);
            }

            return sb;
        }
    }
}
