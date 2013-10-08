using System.Collections.Generic;
using JFlacLib.JFlac.Metadata;
using JFlacLib.JFlac.Util;

namespace JFlacLib.JFlac
{
    class PCMProcessors:IPCMProcessor
    {

        private readonly HashSet<IPCMProcessor> _pcmProcessors = new HashSet<IPCMProcessor>();

        public void AddPCMProcessor(IPCMProcessor processor)
        {
            lock (_pcmProcessors)
            {
                if (!_pcmProcessors.Contains(processor))
                    _pcmProcessors.Add(processor);
            }
        }

        /**
         * Remove a PCM processor.
         * @param processor  The processor listener to remove
         */
        public void RemovePCMProcessor(IPCMProcessor processor)
        {
            lock (_pcmProcessors)
            {
                if (_pcmProcessors.Contains(processor))
                    _pcmProcessors.Remove(processor);
            }
        }

        public void ProcessStreamInfo(ref StreamInfo streamInfo)
        {
            lock (_pcmProcessors)
            {
                var it = _pcmProcessors.GetEnumerator();
                while (it.MoveNext())
                {
                    var processor = it.Current;
                    if (processor != null) processor.ProcessStreamInfo(ref streamInfo);
                }
            }
        }

        public void ProcessPCM(ByteData pcm)
        {
            lock (_pcmProcessors)
            {
                var it = _pcmProcessors.GetEnumerator();
                while (it.MoveNext())
                {
                    var processor = it.Current;
                    if (processor != null) processor.ProcessPCM(pcm);
                }
            }
        }
    }
}
