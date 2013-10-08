using System;
using System.Text;
using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Metadata
{
    public class Picture : Metadata
    {
        private int pictureType;
        private int mimeTypeByteCount;
        private String mimeString; //ASCII 0x20 to 0x7e or --> (data is URL)
        private int descStringByteCount;
        private String descString; // UTF-8
        private int picPixelWidth;
        private int picPixelHeight;
        private int picBitsPerPixel;
        private int picColorCount; // for GIF, else 0
        private int picByteCount;

        protected byte[] image;

        /**
         * The constructor.
         * @param is                The InputBitStream
         * @param length            Length of the record
         * @param isLast            True if this is the last Metadata block in the chain
         * @throws IOException      Thrown if error reading from InputBitStream
         */

        public Picture(BitInputStream inputStream, int length, bool isLast)
            : base(isLast)
        {
            int usedBits = 0;
            byte[] data;

            pictureType = inputStream.ReadRawUInt(32);
            usedBits += 32;

            mimeTypeByteCount = inputStream.ReadRawUInt(32);
            usedBits += 32;

            data = new byte[mimeTypeByteCount];
            inputStream.ReadByteBlockAlignedNoCRC(data, mimeTypeByteCount);
            usedBits += mimeTypeByteCount*8;

            mimeString = Encoding.Default.GetString(data); // convert to a string

            descStringByteCount = inputStream.ReadRawUInt(32);
            usedBits += 32;

            if (descStringByteCount != 0)
            {
                data = new byte[descStringByteCount];
                inputStream.ReadByteBlockAlignedNoCRC(data, descStringByteCount);
                try
                {
                    descString = Encoding.UTF8.GetString(data);
                }
                catch (DecoderFallbackException)
                {
                }
                usedBits += 32;
            }
            else
            {
                descString = "";
            }

            picPixelWidth = inputStream.ReadRawUInt(32);
            usedBits += 32;

            picPixelHeight = inputStream.ReadRawUInt(32);
            usedBits += 32;

            picBitsPerPixel = inputStream.ReadRawUInt(32);
            usedBits += 32;

            picColorCount = inputStream.ReadRawUInt(32);
            usedBits += 32;

            picByteCount = inputStream.ReadRawUInt(32);
            usedBits += 32;

            //get the image now
            image = new byte[picByteCount];
            inputStream.ReadByteBlockAlignedNoCRC(image, picByteCount);
            usedBits += picByteCount*8;

            // skip the rest of the block if any
            length -= (usedBits/8);
            inputStream.ReadByteBlockAlignedNoCRC(null, length);

        }

        /**
         * Convert the class to a string representation.
         * @return  A string representation of the Picture metadata
         */


        public override String ToString()
        {
            return "Picture: "
                   + " Type=" + pictureType
                   + " MIME type=" + mimeString
                   + " Description=\"" + descString + "\""
                   + " Pixels (WxH)=" + picPixelWidth + "x" + picPixelHeight
                   + " Color Depth=" + picBitsPerPixel
                   + " Color Count=" + picColorCount
                   + " Picture Size (bytes)=" + picByteCount;
        }
    }
}

