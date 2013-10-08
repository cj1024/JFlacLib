namespace JFlacLib.JFlac.Util
{
    public class ByteData
    {
        private const int DEFAULT_BUFFER_SIZE = 256;

        /** The byte array where data is stored. */
        internal byte[] Data { get; set; }

        /** The number of bytes stored in the array. */
        internal int Length { get; set; }

        /**
         * The default constructor.
         * @param maxSpace  The maximum space in the internal byte array.
         */
        public ByteData(int maxSpace)
        {
            if (maxSpace <= 0) maxSpace = DEFAULT_BUFFER_SIZE;
            Data = new byte[maxSpace];
            Length = 0;
        }


        /**
         * Append byte to storage.
         * @param b byte to extend
         */
        public void Append(byte b)
        {
            Data[Length++] = b;
        }


        public byte GetData(int idx)
        {
            return Data[idx];
        }

        public void SetLength(int len)
        {
            if (len > Data.Length)
            {
                len = Data.Length;
            }
            Length = len;
        }

    }
}
