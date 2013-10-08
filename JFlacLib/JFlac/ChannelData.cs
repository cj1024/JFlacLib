using JFlacLib.JFlac.Frame;

namespace JFlacLib.JFlac
{
    public class ChannelData
    {
        public int[] Output { get; set; }

        public int[] Residual { get; set; }

        public EntropyPartitionedRiceContents PartitionedRiceContents { get; private set; }

        public ChannelData(int size)
        {
            Output = new int[size];
            Residual = new int[size];
            PartitionedRiceContents = new EntropyPartitionedRiceContents();
        }

    }
}
