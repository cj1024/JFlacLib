using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Metadata
{
    public class Unknown : Metadata
    {
        protected byte[] Data;

        public Unknown(BitInputStream inputStream, int length, bool isLast)
            : base(isLast)
        {
            if (length > 0)
            {
                Data = new byte[length];
                inputStream.ReadByteBlockAlignedNoCRC(Data, length);
            }
        }
    }
}
