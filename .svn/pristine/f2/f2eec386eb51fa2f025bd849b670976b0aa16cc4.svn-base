using System;
using System.Diagnostics;
using System.IO;
using JFlacLib.JFlac.Metadata;
using JFlacLib.JFlac.Util;

namespace JFlacLib.JFlac.Apps
{
    public class Decoder : IPCMProcessor
    {
        private WavWriter _wavWriter;

        public void Decode(String inFileName, String outFileName)
        {
            Stream ins = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            if (File.Exists(outFileName))
                File.Delete(outFileName);
            Stream ous = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            Decode(ins,ous);
        }

        public void Decode(Stream inputStream,Stream outputStream)
        {
            var stopwatch=new Stopwatch();
            stopwatch.Start();
            _wavWriter = new WavWriter(outputStream);
            var decoder = new FLACDecoder(inputStream);
            decoder.AddPCMProcessor(this);
            decoder.Decode();
            inputStream.Close();
            outputStream.Close();
            inputStream.Dispose();
            outputStream.Dispose();
            stopwatch.Stop();
            Debug.Write(stopwatch.ElapsedMilliseconds);
        }
        
        public void ProcessStreamInfo(ref StreamInfo info)
        {
            try
            {
                _wavWriter.WriteHeader(info);
            }
            catch (IOException)
            {

            }
        }

        public void ProcessPCM(ByteData pcm)
        {
            try
            {
                _wavWriter.WritePCM(pcm);
            }
            catch (IOException)
            {

            }
        }

    }

}
