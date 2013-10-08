using System;
using JFlacLib.JFlac.IO;
using JFlacLib.JFlac.Metadata;
using JFlacLib.JFlac.Util;

namespace JFlacLib.JFlac.Frame
{
    public class Header
    {
        /** The number of samples per subframe. */
    public int blockSize;
    /** The sample rate in Hz. */
    public int sampleRate;
    /** The number of channels (== number of subframes). */
    public int channels;
    /** The channel assignment for the frame. */
    public int channelAssignment;
    /** The sample resolution. */
    public int bitsPerSample;
    
    /** 
     * The frame number or sample number of first sample in frame.
     * use the number_type value to determine which to use. 
     */
    public int frameNumber = -1;
    
    /**
     * The sample number for the first sample in the frame.
     */
    public long sampleNumber = -1;
    
    /** 
     * CRC-8 (polynomial = x^8 + x^2 + x^1 + x^0, initialized with 0).
     * of the raw frame header bytes, meaning everything before the CRC byte
     * including the sync code.
     */
    protected byte crc;
    
    /**
     * The constructor.
     * @param is                    The InputBitStream
     * @param headerWarmup          The header warm-up bytes
     * @param streamInfo            The FLAC Stream Info
     * @throws IOException          Thrown on error reading InputBitStream
     * @throws BadHeaderException   Thrown if header is bad
     */
    public Header(BitInputStream inputStream, byte[] headerWarmup, StreamInfo streamInfo){
        int blocksizeHint = 0;
        int sampleRateHint = 0;
        ByteData rawHeader = new ByteData(16); // MAGIC NUMBER based on the maximum frame header size, including CRC
        bool isKnownVariableBlockSizeStream = (streamInfo != null && streamInfo.MinBlockSize != streamInfo.MaxBlockSize);
        bool isKnownFixedBlockSizeStream = (streamInfo != null && streamInfo.MinBlockSize == streamInfo.MaxBlockSize);
        
        // init the raw header with the saved bits from synchronization
        rawHeader.Append(headerWarmup[0]);
        rawHeader.Append(headerWarmup[1]);
        
        // check to make sure that the reserved bits are 0
        if ((rawHeader.GetData(1) & 0x03) != 0) { // MAGIC NUMBER
            throw new BadHeaderException("Bad Magic Number: " + (rawHeader.GetData(1) & 0xff));
        }
        
        // Note that along the way as we read the header, we look for a sync
        // code inside.  If we find one it would indicate that our original
        // sync was bad since there cannot be a sync code in a valid header.
        
        // read in the raw header as bytes so we can CRC it, and parse it on the way
        for (int i = 0; i < 2; i++) {
            if (inputStream.PeekRawUInt(8) == 0xff) { // MAGIC NUMBER for the first 8 frame sync bits
                throw new BadHeaderException("Found sync byte");
            }
            rawHeader.Append((byte) inputStream.ReadRawUInt(8));
        }
        
        int bsType = (rawHeader.GetData(2) >> 4) & 0x0f;
        switch (bsType) {
            case 0 :
                if (!isKnownFixedBlockSizeStream)
                    throw new BadHeaderException("Unknown Block Size (0)");
                blockSize = streamInfo.MinBlockSize;
                break;
            case 1 :
                blockSize = 192;
                break;
            case 2 :
            case 3 :
            case 4 :
            case 5 :
                blockSize = 576 << (bsType - 2);
                break;
            case 6 :
            case 7 :
                blocksizeHint = bsType;
                break;
            case 8 :
            case 9 :
            case 10 :
            case 11 :
            case 12 :
            case 13 :
            case 14 :
            case 15 :
                blockSize = 256 << (bsType - 8);
                break;
            default :
                break;
        }
        //System.out.println("BSType="+bsType+" BS="+blockSize);
        
        int srType = rawHeader.GetData(2) & 0x0f;
        switch (srType) {
            case 0 :
                if (streamInfo == null)
                    throw new BadHeaderException("Bad Sample Rate (0)");
                sampleRate = streamInfo.SampleRate;
                break;
            case 1 :
            case 2 :
            case 3 :
                throw new BadHeaderException("Bad Sample Rate (" + srType + ")");
            case 4 :
                sampleRate = 8000;
                break;
            case 5 :
                sampleRate = 16000;
                break;
            case 6 :
                sampleRate = 22050;
                break;
            case 7 :
                sampleRate = 24000;
                break;
            case 8 :
                sampleRate = 32000;
                break;
            case 9 :
                sampleRate = 44100;
                break;
            case 10 :
                sampleRate = 48000;
                break;
            case 11 :
                sampleRate = 96000;
                break;
            case 12 :
            case 13 :
            case 14 :
                sampleRateHint = srType;
                break;
            case 15 :
                throw new BadHeaderException("Bad Sample Rate (" + srType + ")");
            default :
                break;
        }
        
        int asgnType = (int) ((rawHeader.GetData(3) >> 4) & 0x0f);
        //System.out.println("AsgnType="+asgnType+" "+(rawHeader.space[3] >> 4));
        if ((asgnType & 8) != 0) {
            channels = 2;
            switch (asgnType & 7) {
                case 0 :
                    channelAssignment = Constants.CHANNEL_ASSIGNMENT_LEFT_SIDE;
                    break;
                case 1 :
                    channelAssignment = Constants.CHANNEL_ASSIGNMENT_RIGHT_SIDE;
                    break;
                case 2 :
                    channelAssignment = Constants.CHANNEL_ASSIGNMENT_MID_SIDE;
                    break;
                default :
                    throw new BadHeaderException("Bad Channel Assignment (" + asgnType + ")");
            }
        } else {
            channels = (int) asgnType + 1;
            channelAssignment = Constants.CHANNEL_ASSIGNMENT_INDEPENDENT;
        }
        
        int bpsType = (int) (rawHeader.GetData(3) & 0x0e) >> 1;
        switch (bpsType) {
            case 0 :
                if (streamInfo != null)
                    bitsPerSample = streamInfo.BitsPerSample;
                else
                    throw new BadHeaderException("Bad BPS (" + bpsType + ")");
                break;
            case 1 :
                bitsPerSample = 8;
                break;
            case 2 :
                bitsPerSample = 12;
                break;
            case 4 :
                bitsPerSample = 16;
                break;
            case 5 :
                bitsPerSample = 20;
                break;
            case 6 :
                bitsPerSample = 24;
                break;
            case 3 :
            case 7 :
                throw new BadHeaderException("Bad BPS (" + bpsType + ")");
            default :
                break;
        }
        
        if ((rawHeader.GetData(3) & 0x01) != 0) { // this should be a zero padding bit
            throw new BadHeaderException("this should be a zero padding bit");
        }
        
        if ((blocksizeHint != 0) && isKnownVariableBlockSizeStream) {
            sampleNumber = inputStream.ReadUTF8Long(rawHeader);
            if (sampleNumber == 0xfffffffffffffffL) { // i.e. non-UTF8 code...
                throw new BadHeaderException("Bad Sample Number");
            }
        } else {
            int lastFrameNumber = inputStream.ReadUTF8Int(rawHeader);
            if (lastFrameNumber == 0xfffffff) { // i.e. non-UTF8 code...
                throw new BadHeaderException("Bad Last Frame");
            }
            sampleNumber = (long) streamInfo.MinBlockSize * (long) lastFrameNumber;
        }
        
        if (blocksizeHint != 0) {
            int blockSizeCode = inputStream.ReadRawUInt(8);
            rawHeader.Append((byte) blockSizeCode);
            if (blocksizeHint == 7) {
                int blockSizeCode2 = inputStream.ReadRawUInt(8);
                rawHeader.Append((byte) blockSizeCode2);
                blockSizeCode = (blockSizeCode << 8) | blockSizeCode2;
            }
            blockSize = blockSizeCode + 1;
        }
        
        if (sampleRateHint != 0) {
            int sampleRateCode = inputStream.ReadRawUInt(8);
            rawHeader.Append((byte) sampleRateCode);
            if (sampleRateHint != 12) {
                int sampleRateCode2 = inputStream.ReadRawUInt(8);
                rawHeader.Append((byte) sampleRateCode2);
                sampleRateCode = (sampleRateCode << 8) | sampleRateCode2;
            }
            if (sampleRateHint == 12)
                sampleRate = sampleRateCode * 1000;
            else if (sampleRateHint == 13)
                sampleRate = sampleRateCode;
            else
                sampleRate = sampleRateCode * 10;
        }
        
        // read the CRC-8 byte
        byte crc8 = (byte)inputStream.ReadRawUInt(8);
        
        if (CRC8.Calc(rawHeader.Data, rawHeader.Length) != crc8) {
            throw new BadHeaderException("STREAM_DECODER_ERROR_STATUS_BAD_HEADER");
        }
    }
    
    /**
     * Return a descriptive string for this object.
     * @return the string description
     * @see java.lang.Object#toString()
     */
        
    public override String ToString() {
        return "FrameHeader:"
            + " BlockSize=" + blockSize
            + " SampleRate=" + sampleRate
            + " Channels=" + channels
            + " ChannelAssignment=" + channelAssignment
            + " BPS=" + bitsPerSample
            + " SampleNumber=" + sampleNumber;
    }
    }
}
