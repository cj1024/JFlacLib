using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPJFlacLib.JFlac;
using WPJFlacLib.JFlac.Frame;
using WPJFlacLib.JFlac.Metadata;
using WPJFlacLib.JFlac.Util;

namespace AudioStreamSource.Flac
{
    public class FlacMediaStreamSource : MediaStreamSource
    {
        public const string ImagePath = "/Shared/Media/TEMPCOVER.jpg";

        private readonly Stream _inputStream;

        private StreamInfo _streamInfo;

        private readonly FLACDecoder _decoder;

        public event EventHandler StreamComplete;

        public FlacMediaStreamSource(Stream iStream)
        {
            _inputStream = iStream;
            _decoder = new FLACDecoder(_inputStream);
        }

        #region Override

        protected override void OpenMediaAsync()
        {
            ReadPastId3V2Tags(ReadPastId3V2TagsCallback);
        }

        protected override void SeekAsync(long seekToTime)
        {
            ReportSeekCompleted(seekToTime);
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            var emptyDict = new Dictionary<MediaSampleAttributeKeys, string>();
            MediaStreamSample audioSample;
            try
            {

                Frame frame = _decoder.ReadFrame(true);
                if (frame != null)
                {
                    // Calculate our current position instead based on the bitrate of the stream (more accurate?)
                    double position = frame.Header.SampleNumber * 1d / _streamInfo.SampleRate;
                    TimeSpan currentPosition = TimeSpan.FromSeconds(position);

                    // Create a MemoryStream to hold the bytes
                    // FrameSize includes the frame header which we've already read from the previous iteration, so just copy the
                    // header, and then read the remaining bytes
                    var bd = _decoder.DecodeFrame(frame, null);
                    using (var audioFrameStream = new MemoryStream(bd.Data, 0, bd.Length))
                    {
                        // Return the next sample in the stream
                        audioSample = new MediaStreamSample(_audioStreamDescription, audioFrameStream, 0, audioFrameStream.Length, currentPosition.Ticks, emptyDict);
                        ReportGetSampleCompleted(audioSample);
                    }
                }
                else
                {
                    // We're near the end of the file, or we got an irrecoverable error.
                    // Return a null stream which tells the MediaStreamSource & MediaElement to shut down
                    audioSample = new MediaStreamSample(_audioStreamDescription, null, 0, 0, 0, emptyDict);
                    ReportGetSampleCompleted(audioSample);
                }
            }
            catch (EndOfStreamException)
            {
                audioSample = new MediaStreamSample(_audioStreamDescription, null, 0, 0, 0, emptyDict);
                ReportGetSampleCompleted(audioSample);
            }
        }

        /// <summary>
        /// Don't Know How To Implement;
        /// </summary>
        /// <param name="mediaStreamDescription"></param>
        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Don't Know How To Implement;
        /// </summary>
        /// <param name="diagnosticKind"> </param>
        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        protected override void CloseMedia()
        {
            try
            {
                _inputStream.Close();
                CallStreamComplete();
            }
            catch (CryptographicException)
            {
                // Ignore these, they are thrown when abruptly closing a
                // stream (i.e. skipping tracks) where the source is a
                // CryptoStream
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.StackTrace);
            }
        }

        #endregion

        private MediaStreamDescription _audioStreamDescription;

        public void ReadPastId3V2Tags(Action<StreamInfo> callback)
        {
            var metadatas = _decoder.ReadMetadata();
            var streamInfo = metadatas.FirstOrDefault(s => s is StreamInfo) as StreamInfo;
            var pic = metadatas.FirstOrDefault(s => s is Picture) as Picture;
            if (pic != null)
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        using (var stream = new MemoryStream(pic.Image))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.SetSource(stream);
                            var image = new WriteableBitmap(bitmap);
                            using (var storage=IsolatedStorageFile.GetUserStoreForApplication())
                            {
                                if(!storage.DirectoryExists("/Shared"))
                                {
                                    storage.CreateDirectory("/Shared");
                                }
                                if(!storage.DirectoryExists("/Shared/Media"))
                                {
                                    storage.CreateDirectory("/Shared/Media");
                                }
                                using (var os = new IsolatedStorageFileStream(ImagePath, FileMode.Create, FileAccess.Write, storage))
                                {
                                    image.SaveJpeg(os, image.PixelWidth, image.PixelHeight, 0, 100);
                                    os.Flush();
                                    os.Close();
                                    os.Dispose();
                                }
                            }
                        }

                    });
            if (callback != null)
                callback(streamInfo);
        }

        private void ReadPastId3V2TagsCallback(StreamInfo streamInfo)
        {
            _streamInfo = streamInfo;
            var mediaSourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            var trackDuration = TimeSpan.FromSeconds(streamInfo.TotalSamples * 1d / streamInfo.SampleRate);
            mediaSourceAttributes[MediaSourceAttributesKeys.Duration] = trackDuration.Ticks.ToString(CultureInfo.InvariantCulture);
            if (_inputStream.CanSeek)
            {
                mediaSourceAttributes[MediaSourceAttributesKeys.CanSeek] = "1";
            }
            else
            {
                mediaSourceAttributes[MediaSourceAttributesKeys.CanSeek] = "0";
            }

            // Initialize the Mp3 data structures used by the Media pipeline with state from the first frame.
            var wfx = new WaveFormatEx();
            wfx.SetFromByteArray(WavWriter.GetHeaderBytes(streamInfo));
            var mediaStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            mediaStreamAttributes[MediaStreamAttributeKeys.CodecPrivateData] = wfx.ToHexString();
            _audioStreamDescription = new MediaStreamDescription(MediaStreamType.Audio, mediaStreamAttributes);
            var mediaStreamDescriptions = new Collection<MediaStreamDescription> { _audioStreamDescription };

            // Report that the Mp3MediaStreamSource has finished initializing its internal state and can now
            // pass in Mp3 Samples.
            ReportOpenMediaCompleted(mediaSourceAttributes, mediaStreamDescriptions);
        }

        protected void CallStreamComplete()
        {
            // This may throw a null reference exception - that indicates that the agent did not correctly
            // subscribe to StreamComplete so it could call NotifyComplete
            if (null != StreamComplete)
            {
                StreamComplete(this, new EventArgs());
            }
        }
    }
}
