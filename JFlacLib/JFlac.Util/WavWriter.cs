using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using JFlacLib.JFlac.Metadata;

namespace JFlacLib.JFlac.Util
{
    public class WavWriter
    {
        private const int MAX_BLOCK_SIZE = 65535;

        private long totalSamples;
        private int channels;
        private int bps;
        private int sampleRate;

        private byte[] s8buffer = new byte[MAX_BLOCK_SIZE*Constants.MAX_CHANNELS*4]; /* WATCHOUT: can be up to 2 megs */
        private int samplesProcessed = 0;
        private int frameCounter = 0;

        //private bool needsFixup = false;
        private long riffOffset;
        private long dataOffset;

        private DataOutput os;
        private LittleEndianDataOutput osLE;

        /**
         * The constructor.
         * @param os            The output sream
         * @param streamInfo    The FLAC stream info
         */

        public WavWriter(Stream os, StreamInfo streamInfo)
        {
            this.os = new DataOutput(os);
            osLE = new LittleEndianDataOutput(os);
            totalSamples = streamInfo.TotalSamples;
            channels = streamInfo.Channels;
            bps = streamInfo.BitsPerSample;
            sampleRate = streamInfo.SampleRate;
        }

      

        public WavWriter(Stream os)
        {
            this.os = new DataOutput(os);
            osLE = new LittleEndianDataOutput(os);
        }


        /**
         * Write a WAV file header.
         * @throws IOException  Thrown if error writing to output string.
         */

        public void WriteHeader()
        {
            long dataSize = totalSamples*channels*((bps + 7)/8);
            if (totalSamples == 0)
            {
                //if (!(os instanceof RandomAccessFile)) throw new IOException("Cannot seek in output stream");
                //needsFixup = true;
                throw new FileLoadException("Do not support RandomAccessFile");
            }
            //if (dataSize >= 0xFFFFFFDC) throw new IOException("ERROR: stream is too big to fit in a single file chunk (Datasize="+dataSize+")");
            
            os.Write("RIFF");
            //if (needsFixup) riffOffset = ((System.IO.RandomAccessFile) os).getFilePointer();
            osLE.Write((int) dataSize + 36); // filesize-8
            os.Write("WAVEfmt ");
            os.Write(new byte[] {0x10, 0x00, 0x00, 0x00}); // chunk size = 16
            os.Write(new byte[] { 0x01, 0x00 }); // compression code == 1
            osLE.WriteShort(channels);
            osLE.Write(sampleRate);
            osLE.Write(sampleRate*channels*((bps + 7)/8)); // or is it (sample_rate*channels*bps) / 8
            osLE.WriteShort(channels*((bps + 7)/8)); // block align
            osLE.WriteShort(bps); // bits per sample
            os.Write("data");
            //if (needsFixup) dataOffset = ((RandomAccessFile) os).getFilePointer();

            osLE.Write((int)dataSize); // data size
            os.Flush();
            osLE.Flush();
        }

        /**
         * Write a WAV file header.
         * @param streamInfo    The FLAC stream info
         * @throws IOException  Thrown if error writing to output string.
         */

        public void WriteHeader(StreamInfo streamInfo)
        {
            totalSamples = streamInfo.TotalSamples;
            channels = streamInfo.Channels;
            bps = streamInfo.BitsPerSample;
            sampleRate = streamInfo.SampleRate;
            WriteHeader();
        }

        /**
         * Write a WAV frame record.
         * @param frame         The FLAC frame
         * @param channelData   The decoded channel data
         * @throws IOException  Thrown if error writing to output channel
         */

        public void WriteFrame(Frame.Frame frame, ChannelData[] channelData)
        {
            bool isUnsignedSamples = bps <= 8;
            int wideSamples = frame.Header.blockSize;
            int wideSample;
            int sample;
            int channel;

            if (wideSamples > 0)
            {
                samplesProcessed += wideSamples;
                frameCounter++;
                if (bps == 8)
                {
                    if (isUnsignedSamples)
                    {
                        for (sample = wideSample = 0; wideSample < wideSamples; wideSample++)
                            for (channel = 0; channel < channels; channel++)
                            {
                                //System.out.print("("+(int)((byte)(channelData[channel].getOutput()[wideSample] + 0x80))+")");
                                s8buffer[sample++] = (byte) (channelData[channel].Output[wideSample] + 0x80);
                            }
                    }
                    else
                    {
                        for (sample = wideSample = 0; wideSample < wideSamples; wideSample++)
                            for (channel = 0; channel < channels; channel++)
                                s8buffer[sample++] = (byte) (channelData[channel].Output[wideSample]);
                    }
                    os.Write(s8buffer,0,sample);
                }
                else if (bps == 16)
                {
                    if (isUnsignedSamples)
                    {
                        for (sample = wideSample = 0; wideSample < wideSamples; wideSample++)
                            for (channel = 0; channel < channels; channel++)
                            {
                                short val = (short) (channelData[channel].Output[wideSample] + 0x8000);
                                s8buffer[sample++] = (byte) (val & 0xff);
                                s8buffer[sample++] = (byte) ((val >> 8) & 0xff);
                            }
                    }
                    else
                    {
                        for (sample = wideSample = 0; wideSample < wideSamples; wideSample++)
                            for (channel = 0; channel < channels; channel++)
                            {
                                short val = (short) (channelData[channel].Output[wideSample]);
                                s8buffer[sample++] = (byte) (val & 0xff);
                                s8buffer[sample++] = (byte) ((val >> 8) & 0xff);
                            }
                    }
                    os.Write(s8buffer, 0, sample);
                }
                else if (bps == 24)
                {
                    if (isUnsignedSamples)
                    {
                        for (sample = wideSample = 0; wideSample < wideSamples; wideSample++)
                            for (channel = 0; channel < channels; channel++)
                            {
                                int val = (channelData[channel].Output[wideSample] + 0x800000);
                                s8buffer[sample++] = (byte) (val & 0xff);
                                s8buffer[sample++] = (byte) ((val >> 8) & 0xff);
                                s8buffer[sample++] = (byte) ((val >> 16) & 0xff);
                            }
                    }
                    else
                    {
                        for (sample = wideSample = 0; wideSample < wideSamples; wideSample++)
                            for (channel = 0; channel < channels; channel++)
                            {
                                int val = (channelData[channel].Output[wideSample]);
                                s8buffer[sample++] = (byte) (val & 0xff);
                                s8buffer[sample++] = (byte) ((val >> 8) & 0xff);
                                s8buffer[sample++] = (byte) ((val >> 16) & 0xff);
                            }
                    }
                    os.Write(s8buffer, 0, sample);
                }
            }
        }

        public void WritePCM(ByteData space)
        {
            os.Write(space.Data,0,space.Length);
        }
    }
}
