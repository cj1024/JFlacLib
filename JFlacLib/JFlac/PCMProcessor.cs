using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JFlacLib.JFlac.Metadata;
using JFlacLib.JFlac.Util;

namespace JFlacLib.JFlac
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
