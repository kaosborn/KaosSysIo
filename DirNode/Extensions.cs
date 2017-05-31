using System.Text;

namespace Kaos.SysIo
{
    public static class Extensions
    {
        public static StringBuilder AppendIndent (this StringBuilder sb, DirNode.Vector dv, bool forFiles)
        {
            var topIx = dv.Items.Count - 1;
            if (! forFiles && dv.Items[topIx].Index < 0)
                --topIx;

            for (var ix = 1; ix <= topIx; ++ix)
                if (! forFiles && ix == topIx)
                {
                    if (dv.Items[ix].IsLast)
                        sb.Append (dv.UpRight);
                    else
                        sb.Append (dv.UpDownRight);
                    for (int kk = 1; kk < dv.TabSize; ++kk)
                        sb.Append (dv.LeftRight);
                }
                else
                {
                    if (dv.Items[ix].IsLast)
                        sb.Append (' ');
                    else
                        sb.Append (dv.UpDown);
                    for (int kk = 1; kk < dv.TabSize; ++kk)
                        sb.Append (' ');
                }

            return sb;
        }
    }
}
