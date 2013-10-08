using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPJFlacLib.JFlac.Apps
{
    public class Analyzer : IFrameListener
    {
        private int _frameNum = 0;

        public Metadata.Metadata[] Analysis(string inFileName)
        {
            var inputStream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(inFileName,FileMode.Open,FileAccess.Read);
            return Analysis(inputStream);
        }

        public Metadata.Metadata[] Analysis(Stream inputStream)
        {
            var decoder = new FLACDecoder(inputStream);
            decoder.AddFrameListener(this);
            var result= decoder.ReadMetadata();
            inputStream.Close();
            inputStream.Dispose();
            return result;
        }

        public void ProcessMetadata(Metadata.Metadata metadata)
        {

        }

        public void ProcessFrame(ref Frame.Frame frame)
        {
            _frameNum++;
            Debug.WriteLine(_frameNum + " " + frame);
        }

        public void ProcessError(string msg)
        {
            Debug.WriteLine(msg);
        }
    }
}
