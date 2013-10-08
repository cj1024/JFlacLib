using WPJFlacLib.JFlac.Metadata;
using WPJFlacLib.JFlac.Util;

namespace WPJFlacLib.JFlac
{
    public interface IPCMProcessor
    {
        void ProcessStreamInfo(ref StreamInfo streamInfo);

        /**
         * Called when each data frame is decompressed.
         * @param pcm The decompressed PCM data
         */
        void ProcessPCM(ByteData pcm);
    }
}