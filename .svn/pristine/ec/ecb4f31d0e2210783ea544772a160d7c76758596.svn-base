using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CJToolkit.Util;

namespace CJToolkit.ImageExtention
{
    [TemplatePart(Name = RootElement, Type = typeof(Grid))]
    [TemplatePart(Name = GifBrush, Type = typeof(ImageBrush))]
    public class FakeGifBase:Control
    {
        #region TemplatePart Name

        private const String RootElement = "RootElement";
        private const String GifBrush = "GifBrush";

        #endregion

        #region TemplatePart

        private Grid _rootELement;
        private ImageBrush _gifBrush;

        #endregion

        #region 私有属性

        private DispatcherTimer GifTimer { get; set; }

        private int TotalFrameCount
        {
            get { return ImageSource == null ? 0 : ImageSource.Count(); }
        }

        public int CurrentFrame { get; private set; }

        #endregion

        #region Constructor

        internal FakeGifBase()
        {
            DefaultStyleKey = typeof (FakeGifBase);
        }

        ~FakeGifBase()
        {
            ThreadUtil.TryStopTimer(GifTimer);
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootELement = GetTemplateChild(RootElement) as Grid;
            _gifBrush = GetTemplateChild(GifBrush) as ImageBrush;
        }

        #endregion

        #region 依赖属性

        #region GIF图片源

        protected static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof (IEnumerable<ImageSource>), typeof (FakeGifBase),
                                        new PropertyMetadata(default(IEnumerable<BitmapImage>), OnImageSourceChanged));

        protected IEnumerable<ImageSource> ImageSource
        {
            get { return (IEnumerable<ImageSource>) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private static void OnImageSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FakeGifBase)obj).ImageSourceChanged();
        }

        #endregion

        #region 拉伸方式

        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.Register("ImageStretch", typeof (Stretch), typeof (FakeGifBase),
                                        new PropertyMetadata(Stretch.Fill));

        public Stretch ImageStretch
        {
            get { return (Stretch) GetValue(ImageStretchProperty); }
            set { SetValue(ImageStretchProperty, value); }
        }

        #endregion

        #region 播放帧率

        public static readonly DependencyProperty FramePerSecondProperty =
            DependencyProperty.Register("FramePerSecond", typeof (int), typeof (FakeGifBase),
                                        new PropertyMetadata(30, OnFramePerSecondChanged));

        public int FramePerSecond
        {
            get { return (int) GetValue(FramePerSecondProperty); }
            set { SetValue(FramePerSecondProperty, value); }
        }

        private static void OnFramePerSecondChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if ((int) e.NewValue < 1)
                throw new ArgumentOutOfRangeException("e", "FramePerSecond should be a positive integer");
            ((FakeGifBase) obj).FramePerSecondChanged();
        }

        #endregion

        #endregion

        #region 私有方法

        #region 依赖属性变化相关

        private void ImageSourceChanged()
        {
            RealStart();
        }

        private void FramePerSecondChanged()
        {
            if (GifTimer != null)
                GifTimer.Interval = TimeSpan.FromMilliseconds(1000d/FramePerSecond);
        }

        #endregion

        #region 动画播放相关

        private void RealStart()
        {
            if(FramePerSecond<1)
            {
                NotifyMessage.NotifyMessage.Show("FPS设置有误");
                return;
            }
            if (GifTimer != null) GifTimer.Stop();
            CurrentFrame = -1;
            GifTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1000d/FramePerSecond)};
            GifTimer.Tick += PlayNextFrame;
            GifTimer.Start();
        }

        private void RealStop()
        {
            if (GifTimer != null) GifTimer.Stop();
            CurrentFrame = -1;
        }

        private void RealPause()
        {
            if (GifTimer != null) GifTimer.Stop();
        }

        private void RealResume()
        {
            if (GifTimer != null) GifTimer.Start();
        }

        private void PlayNextFrame(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (_gifBrush == null) return;
                    if (ImageSource == null) return;
                    if (TotalFrameCount < 1) return;
                    CurrentFrame = (CurrentFrame + 1)%TotalFrameCount;
                    _gifBrush.ImageSource = ImageSource.ElementAt(CurrentFrame);
                    _gifBrush.Stretch = ImageStretch;
                });
        }

        #endregion

        #endregion

        #region 公开方法

        public void Start()
        {
            RealStart();
        }

        public void Stop()
        {
            RealStop();
        }

        public void Pause()
        {
            RealPause();
        }

        public void Resume()
        {
            RealResume();
        }

        #endregion


    }
}
