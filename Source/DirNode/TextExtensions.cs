using System.Text;

namespace Kaos.SysIo
{
    /// <summary>Define extension methods for the *System.Text* namespace.</summary>
    public static class TextExtensions
    {
        /// <summary>Appends an appropriate number of spaces to this instance.</summary>
        /// <param name="sb">Instance to append to.</param>
        /// <param name="dv">Path to a directory node.</param>
        /// <param name="forFiles">*true* if files are output, else *false*.</param>
        /// <returns>The <see cref="StringBuilder"/> instance.</returns>
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
