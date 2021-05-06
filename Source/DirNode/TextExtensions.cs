using System.Text;

namespace Kaos.SysIo
{
    public static class TextExtensions
    {
        public static StringBuilder AppendIndent (this StringBuilder sb, DirNode.Vector dv, bool forFiles)
        {
            var topIx = dv.Count - 1;
            if (! forFiles && dv[topIx].Index < 0)
                --topIx;

            for (var ix = 1; ix <= topIx; ++ix)
                if (! forFiles && ix == topIx)
                {
                    sb.Append (dv[ix].IsLast ? dv.UpRight : dv.UpDownRight);
                    for (var jx = 1; jx < dv.TabSize; ++jx)
                        sb.Append (dv.LeftRight);
                }
                else
                {
                    sb.Append (dv[ix].IsLast ? ' ' : dv.UpDown);
                    for (var jx = 1; jx < dv.TabSize; ++jx)
                        sb.Append (' ');
                }

            return sb;
        }
    }
}
