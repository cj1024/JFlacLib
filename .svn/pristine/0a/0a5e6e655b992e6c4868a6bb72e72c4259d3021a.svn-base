using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Metadata
{
    public class CueTrack
    {
        private const int CuesheetTrackOffsetLen = 64; // bits
        private const int CuesheetTrackNumberLen = 8; // bits
        private const int CuesheetTrackIsrcLen = 12*8; // bits
        private const int CuesheetTrackTypeLen = 1; // bit
        private const int CuesheetTrackPreEmphasisLen = 1; // bit
        private const int CuesheetTrackReservedLen = 6 + 13*8; // bits
        private const int CuesheetTrackNumIndicesLen = 8; // bits

        internal long Offset; // Track offset in samples, relative to the beginning of the FLAC audio stream.
        internal byte Number; // The track number.
        internal byte[] Isrc = new byte[13]; // Track ISRC.  This is a 12-digit alphanumeric code plus a trailing '\0'
        internal int Type; // The track type: 0 for audio, 1 for non-audio.
        internal int PreEmphasis; // The pre-emphasis flag: 0 for no pre-emphasis, 1 for pre-emphasis.
        internal byte NumIndices; // The number of track index points.
        internal CueIndex[] Indices; // NULL if num_indices == 0, else pointer to array of index points.

        public CueTrack(BitInputStream inputStream)
        {
            Offset = inputStream.ReadRawULong(CuesheetTrackOffsetLen);
            Number = (byte) inputStream.ReadRawUInt(CuesheetTrackNumberLen);
            inputStream.ReadByteBlockAlignedNoCRC(Isrc, CuesheetTrackIsrcLen/8);
            Type = inputStream.ReadRawUInt(CuesheetTrackTypeLen);
            PreEmphasis = inputStream.ReadRawUInt(CuesheetTrackPreEmphasisLen);
            inputStream.SkipBitsNoCRC(CuesheetTrackReservedLen);
            NumIndices = (byte) inputStream.ReadRawUInt(CuesheetTrackNumIndicesLen);
            if (NumIndices > 0)
            {
                Indices = new CueIndex[NumIndices];
                for (int j = 0; j < NumIndices; j++)
                {
                    Indices[j] = new CueIndex(inputStream);
                }
            }

        }
    }
}
