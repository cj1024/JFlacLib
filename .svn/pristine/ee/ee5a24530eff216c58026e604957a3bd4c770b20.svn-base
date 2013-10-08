using System.IO;
using System.Text;

namespace JFlacLib.JFlac.Util
{
    public class DataOutput : BinaryWriter
    {
        public DataOutput(Stream stream):base(stream)
        {
            
        }

        public DataOutput(Stream stream,Encoding encoding):base(stream,encoding)
        {
            
        }

        public override void Write(string value)
        {
            base.Write(Encoding.UTF8.GetBytes(value));
        }

        public void WriteShort(int value)
        {
            base.Write((short) value);
        }
    }
}
