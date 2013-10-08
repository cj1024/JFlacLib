using System;
using System.Text;
using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Metadata
{
    public class VorbisString
    {
        protected byte[] Entry;

        public VorbisString(BitInputStream inputStream)
        {
            int elen = inputStream.ReadRawIntLittleEndian();
            if (elen == 0) return;
            Entry = new byte[elen];
            inputStream.ReadByteBlockAlignedNoCRC(Entry, Entry.Length);
        }

        public override String ToString()
        {
            String s;
            try
            {
                s = Encoding.Default.GetString(Entry);
            }
            catch (Exception)
            {
                s = "";
            }
            return s;
        }
    }
}
