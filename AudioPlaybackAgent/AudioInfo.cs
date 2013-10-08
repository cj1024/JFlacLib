using System.Collections.Generic;
using System.Xml.Serialization;
namespace AudioPlaybackAgent
{
    [XmlRootAttribute("AudioInfo")]
    public class AudioInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Tag { get; set; }
    }
    
    [XmlRootAttribute("AudioInfoList")]
    public class AudioInfoList:List<AudioInfo>
    {
        
    }
}
