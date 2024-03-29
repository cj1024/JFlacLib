﻿using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using AudioStreamSource.Flac;
using Microsoft.Phone.BackgroundAudio;

namespace AudioStreamAgent
{
    /// <summary>
    /// 执行逐曲目流式处理以进行播放的后台代理程序
    /// </summary>
    public class AudioTrackStreamer : AudioStreamingAgent
    {
        /// <summary>
        /// 新曲目需要音频解码时调用
        /// (通常因为即将开始播放)
        /// </summary>
        /// <param name="track">
        /// 需要音频流式处理的曲目
        /// </param>
        /// <param name="streamer">
        /// MediaStreamSource 应该附加到其中以开始播放
        /// 的 AudioStreamer 对象
        /// </param>
        /// <remarks>
        /// 若要对曲目调用此方法，请先将 AudioTrack 的 Source 参数设置为 null，
        /// 然后将 BackgroundAudioPlayer 实例属性集的 Track 属性
        /// 设置为 true；
        /// 否则会假定系统将执行所有流式处理
        /// 和解码
        /// </remarks>
        protected override void OnBeginStreaming(AudioTrack track, AudioStreamer streamer)
        {
            //TODO: 将流转化器的 SetSource 属性设置为 MSS 源
            var path = track.Tag;
            if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(path))
            {
                var iStream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(path, FileMode.Open, FileAccess.Read);
                var mss = new FlacMediaStreamSource(iStream);
                mss.StreamComplete += StreamCompleted;
                streamer.SetSource(mss);
            }
        }

        private void StreamCompleted(object sender, EventArgs e)
        {
            ((FlacMediaStreamSource) sender).StreamComplete -= StreamCompleted;
            NotifyComplete();
        }
    }
}
