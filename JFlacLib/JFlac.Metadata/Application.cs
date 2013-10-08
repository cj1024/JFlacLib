using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Metadata
{
    public class Application : Metadata
    {
        private const int ApplicationIDLen = 32; // bits

        private readonly byte[] _id = new byte[4];
        private readonly byte[] _data;

        public Application(BitInputStream inputStream, int length, bool isLast)
            : base(isLast)
        {
            inputStream.ReadByteBlockAlignedNoCRC(_id, ApplicationIDLen/8);
            length -= ApplicationIDLen/8;

            if (length > 0)
            {
                _data = new byte[length];
                inputStream.ReadByteBlockAlignedNoCRC(_data, length);
            }
        }
    }
}
