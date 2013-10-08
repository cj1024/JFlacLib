using System;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.ViewModel;
using WP7FlacPlayer.Util;
using WPJFlacLib.JFlac.Metadata;

namespace WP7FlacPlayer.Model
{
    /// <summary>
    /// 播放列表用Model
    /// </summary>
    public class MusicBriefInfo : NotificationObject
    {
        protected bool Equals(MusicBriefInfo other)
        {
            return Path == other.Path;
        }

        internal string Path { get; set; }

        private SongProperty _songInfo;

        public SongProperty SongInfo
        {
            get { return _songInfo; }
            set
            {
                _songInfo = value;
                RaisePropertyChanged(() => SongInfo);
            }
        }

        private StreamInfo _streamInfo;

        public StreamInfo StreamInfo
        {
            get { return _streamInfo; }
            set
            {
                _streamInfo = value;
                RaisePropertyChanged(() => StreamInfo);
                RaisePropertyChanged(() => Duration);
            }
        }

        private WriteableBitmap _image;

        public WriteableBitmap Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        public TimeSpan Duration { get { return StreamInfo == null ? TimeSpan.FromSeconds(0) : TimeSpan.FromSeconds(StreamInfo.TotalSamples * 1d / StreamInfo.SampleRate); } }

    }
}
