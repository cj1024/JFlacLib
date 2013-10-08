using System;
using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Metadata
{
    public class SeekPoint
    {
        private const int SeekpointSampleNumberLen = 64; // bits
    private const int SeekpointStreamOffsetLen = 64; // bits
    private const int SeekpointFrameSamplesLen = 16; // bits

    internal long SampleNumber { get; private set; } // The sample number of the target frame.
    internal long StreamOffset { get; set; } // The offset, in bytes, of the target frame with respect to beginning of the first frame.
    internal int FrameSamples { get; private set; } // The number of samples in the target frame.
    
    /**
     * The constructor.
     * @param is                The InputBitStream
     * @throws IOException      Thrown if error reading from InputBitStream
     */
    public SeekPoint(BitInputStream inputStream){
        SampleNumber = inputStream.ReadRawULong(SeekpointSampleNumberLen);
        StreamOffset = inputStream.ReadRawULong(SeekpointStreamOffsetLen);
        FrameSamples = inputStream.ReadRawUInt(SeekpointFrameSamplesLen);
    }
    
    /**
     * The constructor.
     * @param sampleNumber  The sample number of the target frame
     * @param streamOffset  The offset, in bytes, of the target frame with respect to beginning of the first frame
     * @param frameSamples  The number of samples in the target frame
     */
    public SeekPoint(long sampleNumber, long streamOffset, int frameSamples) {
        SampleNumber = sampleNumber;
        StreamOffset = streamOffset;
        FrameSamples = frameSamples;
    }
    
    /**
     * Write out an individual seek point.
     * @param os    The output stream
     * @throws IOException  Thrown if error writing data
     */
    public void Write(BitOutputStream os) {

        os.WriteRawULong(SampleNumber, SeekpointSampleNumberLen);
        os.WriteRawULong(StreamOffset, SeekpointStreamOffsetLen);
        os.WriteRawUInt(FrameSamples, SeekpointFrameSamplesLen);
    }
    
    /**
     * @see java.lang.Object#toString()
     */
    public override String ToString() {
        return "sampleNumber=" + SampleNumber + " streamOffset=" + StreamOffset + " frameSamples=" + FrameSamples;
    }
    
    }
}
