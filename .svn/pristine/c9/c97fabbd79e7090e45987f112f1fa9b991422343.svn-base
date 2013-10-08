using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Metadata
{
    public class CueIndex
    {
        private const int CuesheetIndexOffsetLen = 64; // bits
        private const int CuesheetIndexNumberLen = 8; // bits
        private const int CuesheetIndexReservedLen = 3*8; // bits

        internal long Offset; // Offset in samples, relative to the track offset, of the index point.
        internal byte Number; // The index point number.

        public CueIndex(BitInputStream inputStream)
        {
            Offset = inputStream.ReadRawULong(CuesheetIndexOffsetLen);
            Number = (byte) inputStream.ReadRawUInt(CuesheetIndexNumberLen);
            inputStream.SkipBitsNoCRC(CuesheetIndexReservedLen);
        }
    }
}
