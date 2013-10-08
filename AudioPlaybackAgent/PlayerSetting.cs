using System.Xml.Serialization;

namespace AudioPlaybackAgent
{
    [XmlRootAttribute("PlayerSetting")]
    public class PlayerSetting
    {
        public PlayerSetting()
        {
            TrackIndex = -1;
            ShuffleOn = false;
            SingleTrackOn = false;
        }

        public int TrackIndex { get; set; }

        public bool ShuffleOn { get; set; }

        public bool SingleTrackOn { get; set; }
    }
}
