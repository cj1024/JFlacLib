using System;
using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Frame
{
    public class ChannelVerbatim : Channel
    {
        internal int[] Data; // A pointer to verbatim signal.
        
        public ChannelVerbatim(BitInputStream inputStream, Header header,ref ChannelData channelData, int bps,
                               int wastedBits)
            : base(header, wastedBits)
        {


            Data = channelData.Residual;

            for (int i = 0; i < header.blockSize; i++)
            {
                Data[i] = inputStream.ReadRawInt(bps);
            }

            // decode the subframe
            Array.Copy(Data, 0, channelData.Output, 0, header.blockSize);
        }

        /**
         * @see java.lang.Object#toString()
         */

        public override String ToString()
        {
            return "ChannelVerbatim: WastedBits=" + WastedBits;
        }
    }
}
