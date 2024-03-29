﻿using System;

namespace AudioStreamSource.Flac
{
    public class WaveFormatEx
    {
        public const uint SizeOf = 20;

        /// <summary>
        /// The different formats allowable. For now PCM is the only one we support
        /// </summary>
        private const short FormatPCM = 1;

        /// <summary>
        ///  Gets or sets the FormatTag
        /// </summary>
        public short FormatTag
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the number of Channels
        /// </summary>
        public short Channels
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the number of samples per second
        /// </summary>
        public int SamplesPerSec
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the average bytes per second
        /// </summary>
        public int AvgBytesPerSec
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the alignment of the blocks
        /// </summary>
        public short BlockAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of bits per sample (8 or 16)
        /// </summary>
        public short BitsPerSample
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the structure
        /// </summary>
        public int Size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the extension buffer
        /// </summary>
        public byte[] Ext
        {
            get;
            set;
        }

        /// <summary>
        /// Convert a BigEndian string to a LittleEndian string
        /// </summary>
        /// <param name="bigEndianString">A big endian string</param>
        /// <returns>The little endian string</returns>
        public static string ToLittleEndianString(string bigEndianString)
        {
            if (bigEndianString == null)
            {
                return string.Empty;
            }

            char[] bigEndianChars = bigEndianString.ToCharArray();

            // Guard
            if (bigEndianChars.Length % 2 != 0)
            {
                return string.Empty;
            }

            int i, ai, bi, ci, di;
            char a, b, c, d;
            for (i = 0; i < bigEndianChars.Length / 2; i += 2)
            {
                // front byte
                ai = i;
                bi = i + 1;

                // back byte
                ci = bigEndianChars.Length - 2 - i;
                di = bigEndianChars.Length - 1 - i;

                a = bigEndianChars[ai];
                b = bigEndianChars[bi];
                c = bigEndianChars[ci];
                d = bigEndianChars[di];

                bigEndianChars[ci] = a;
                bigEndianChars[di] = b;
                bigEndianChars[ai] = c;
                bigEndianChars[bi] = d;
            }

            return new string(bigEndianChars);
        }

        /// <summary>
        /// Convert the data to a hex string
        /// </summary>
        /// <returns>A string in hexadecimal</returns>
        public string ToHexString()
        {
            string s = string.Empty;

            s += ToLittleEndianString(string.Format("{0:X4}", this.FormatTag));
            s += ToLittleEndianString(string.Format("{0:X4}", this.Channels));
            s += ToLittleEndianString(string.Format("{0:X8}", this.SamplesPerSec));
            s += ToLittleEndianString(string.Format("{0:X8}", this.AvgBytesPerSec));
            s += ToLittleEndianString(string.Format("{0:X4}", this.BlockAlign));
            s += ToLittleEndianString(string.Format("{0:X4}", this.BitsPerSample));
            s += ToLittleEndianString(string.Format("{0:X8}", this.Size));

            return s;
        }

        /// <summary>
        /// Set the data from a byte array (usually read from a file)
        /// </summary>
        /// <param name="byteArray">The array used as input to the stucture</param>
        public void SetFromByteArray(byte[] byteArray)
        {
            if ((byteArray.Length + 2) < SizeOf)
            {
                throw new ArgumentException("Byte array is too small");
            }

            this.FormatTag = BitConverter.ToInt16(byteArray, 0);
            this.Channels = BitConverter.ToInt16(byteArray, 2);
            this.SamplesPerSec = BitConverter.ToInt32(byteArray, 4);
            this.AvgBytesPerSec = BitConverter.ToInt32(byteArray, 8);
            this.BlockAlign = BitConverter.ToInt16(byteArray, 12);
            this.BitsPerSample = BitConverter.ToInt16(byteArray, 14);
            if (byteArray.Length >= SizeOf)
            {
                this.Size = BitConverter.ToInt32(byteArray, 16);
            }
            else
            {
                this.Size = 0;
            }

            if (byteArray.Length > WaveFormatEx.SizeOf)
            {
                this.Ext = new byte[byteArray.Length - WaveFormatEx.SizeOf];
                Array.Copy(byteArray, (int)WaveFormatEx.SizeOf, this.Ext, 0, this.Ext.Length);
            }
            else
            {
                this.Ext = null;
            }
        }

        /// <summary>
        /// Ouput the data into a string.
        /// </summary>
        /// <returns>A string representing the WAVEFORMATEX</returns>
        public override string ToString()
        {
            char[] rawData = new char[18];
            BitConverter.GetBytes(this.FormatTag).CopyTo(rawData, 0);
            BitConverter.GetBytes(this.Channels).CopyTo(rawData, 2);
            BitConverter.GetBytes(this.SamplesPerSec).CopyTo(rawData, 4);
            BitConverter.GetBytes(this.AvgBytesPerSec).CopyTo(rawData, 8);
            BitConverter.GetBytes(this.BlockAlign).CopyTo(rawData, 12);
            BitConverter.GetBytes(this.BitsPerSample).CopyTo(rawData, 14);
            BitConverter.GetBytes(this.Size).CopyTo(rawData, 16);
            return new string(rawData);
        }

        /// <summary>
        /// Calculate the duration of audio based on the size of the buffer
        /// </summary>
        /// <param name="audioDataSize">the buffer size in bytes</param>
        /// <returns>The duration of that buffer</returns>
        public long AudioDurationFromBufferSize(uint audioDataSize)
        {
            if (this.AvgBytesPerSec == 0)
            {
                return 0;
            }

            return (long)audioDataSize * 10000000 / this.AvgBytesPerSec;
        }

        /// <summary>
        /// Calculate the buffer size necessary for a duration of audio
        /// </summary>
        /// <param name="duration">the duration</param>
        /// <returns>the size of the buffer necessary</returns>
        public long BufferSizeFromAudioDuration(long duration)
        {
            long size = duration * this.AvgBytesPerSec / 10000000;
            uint remainder = (uint)(size % this.BlockAlign);
            if (remainder != 0)
            {
                size += this.BlockAlign - remainder;
            }

            return size;
        }

        /// <summary>
        /// Validate that the Wave format is consistent.
        /// </summary>
        public void ValidateWaveFormat()
        {
            if (this.FormatTag != FormatPCM)
            {
                throw new InvalidOperationException("Only PCM format is supported");
            }

            if (this.Channels != 1 && this.Channels != 2)
            {
                throw new InvalidOperationException("Only 1 or 2 channels are supported");
            }

            if (this.BitsPerSample != 8 && this.BitsPerSample != 16)
            {
                throw new InvalidOperationException("Only 8 or 16 bit samples are supported");
            }

            if (this.Size != 0)
            {
                throw new InvalidOperationException("Size must be 0");
            }

            if (this.BlockAlign != this.Channels * (this.BitsPerSample / 8))
            {
                throw new InvalidOperationException("Block Alignment is incorrect");
            }

            if (this.SamplesPerSec > (uint.MaxValue / this.BlockAlign))
            {
                throw new InvalidOperationException("SamplesPerSec overflows");
            }

            if (this.AvgBytesPerSec != this.SamplesPerSec * this.BlockAlign)
            {
                throw new InvalidOperationException("AvgBytesPerSec is wrong");
            }
        }
    }
}
