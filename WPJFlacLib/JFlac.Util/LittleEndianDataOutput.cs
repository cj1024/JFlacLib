using System.IO;

namespace WPJFlacLib.JFlac.Util
{
    internal class LittleEndianDataOutput : DataOutput
    {
        public LittleEndianDataOutput(Stream stream) : base(stream)
        {
        }
    }
}