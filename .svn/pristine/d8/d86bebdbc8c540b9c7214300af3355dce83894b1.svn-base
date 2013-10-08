using System;
using System.IO;

namespace WP7FlacPlayer.Util
{
    public class MemoryAudioDecodeStream : MemoryStream
    {

        private bool _audioDecoded;

        public bool AudioDecoded
        {
            get { return _audioDecoded; }
            set
            {
                _audioDecoded = value;
                if (AudioDecoded && AudioDecodedAction != null)
                {
                    AudioDecodedAction.Invoke();
                }
            }
        }

        public Action AudioDecodedAction { get; set; }

    }
}
