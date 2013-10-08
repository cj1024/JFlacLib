using System;
using System.Text;

namespace JFlacLib.JFlac.Frame
{
    public class Frame
    {
        public Header Header { get; set; }

        private Channel[] _subframes = new Channel[Constants.MAX_CHANNELS];
        public Channel[] SubFrames { get { return _subframes; } set { _subframes = value; } }
        
        public int CRC { get; set; }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Frame Header: " + Header + "\n");
            for (int i = 0; i < Header.channels; i++)
            {
                sb.Append("\tFrame Data " + SubFrames[i] + "\n");
            }
            sb.Append("\tFrame Footer: " + CRC);

            return sb.ToString();

        }

        public static int GetMaxRicePartitionOrderFromBlocksize(int blocksize)
        {
            int maxRicePartitionOrder = 0;
            while ((blocksize & 1) == 0)
            {
                maxRicePartitionOrder++;
                blocksize >>= 1;
            }
            return Math.Min(Constants.MAX_RICE_PARTITION_ORDER, maxRicePartitionOrder);
        }

        
    }
}
