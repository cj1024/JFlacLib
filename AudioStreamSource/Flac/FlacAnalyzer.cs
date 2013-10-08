using System.IO;
using AudioStreamSource.Interface;
using WPJFlacLib.JFlac.Apps;

namespace AudioStreamSource.Flac
{
    public class FlacAnalyzer:IAudioStreamAnaliyzer
    {

        public object Analysis(Stream iStream)
        {
            var analyzer = new Analyzer();
            return analyzer.Analysis(iStream);
        }

    }
}
