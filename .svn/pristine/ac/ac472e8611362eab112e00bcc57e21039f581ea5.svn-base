using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CJToolkit.Util;

namespace CJToolkit.ImageExtention
{
    public class GifBySpiltBase:FakeGifBase
    {
        #region 依赖属性

        #region 帧的宽度

        public static readonly DependencyProperty FrameWidthProperty =
            DependencyProperty.Register("FrameWidth", typeof (int), typeof (GifBySpiltBase),
                                        new PropertyMetadata(-1, OnFrameWidthChanged));

        public int FrameWidth
        {
            get { return (int) GetValue(FrameWidthProperty); }
            set { SetValue(FrameWidthProperty, value); }
        }

        private static void OnFrameWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((GifBySpiltBase)obj).SpiltImage();
        }

        #endregion


        #region 帧的高度

        public static readonly DependencyProperty FrameHeightProperty =
            DependencyProperty.Register("FrameHeight", typeof (int), typeof (GifBySpiltBase),
                                        new PropertyMetadata(-1, OnFrameHeightChanged));

        public int FrameHeight
        {
            get { return (int) GetValue(FrameHeightProperty); }
            set { SetValue(FrameHeightProperty, value); }
        }

        private static void OnFrameHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((GifBySpiltBase)obj).SpiltImage();
        }

        #endregion


        #region 图源

        public static readonly DependencyProperty SourceImageProperty =
            DependencyProperty.Register("SourceImage", typeof (BitmapSource), typeof (GifBySpiltBase),
                                        new PropertyMetadata(default(ImageSource), OnSourceImageChanged));

        public BitmapSource SourceImage
        {
            get { return (BitmapSource) GetValue(SourceImageProperty); }
            set { SetValue(SourceImageProperty, value); }
        }

        private static void OnSourceImageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

            ((GifBySpiltBase)obj).SpiltImage();
        }

        #endregion


        #endregion

        #region 私有方法

        private void SpiltImage()
        {
            if (SourceImage == null || FrameWidth < 0 || FrameHeight < 0) return;
            if (SourceImage is BitmapImage)
            {
                (SourceImage as BitmapImage).CreateOptions = BitmapCreateOptions.DelayCreation;
                (SourceImage as BitmapImage).ImageOpened +=
                    (sender, e) => Deployment.Current.Dispatcher.BeginInvoke(RealSpiltImage);
            }
            else if (SourceImage is WriteableBitmap)
            {

                RealSpiltImage();
            }
        }

        private void RealSpiltImage()
        {
            try
            {
                var w = SourceImage.PixelWidth;
                var h = SourceImage.PixelHeight;
                var spiltWidth = (FrameWidth < 1 || FrameWidth > w) ? w : FrameWidth;
                var spiltHeight = (FrameHeight < 1 || FrameHeight > h) ? h : FrameHeight;
                var spiltCount = h/spiltHeight;
                var source = new WriteableBitmap(SourceImage);
                var list = new List<ImageSource>();
                for (var i = 0; i < spiltCount; i++)
                {
                    var bm = new WriteableBitmap(spiltWidth, spiltHeight);
                    for (var x = 0; x < w; x++)
                    {
                        if (x >= spiltWidth) continue;
                        var start = i * w * spiltHeight;
                        for (var y = 0; y < spiltHeight; y++)
                        {
                            bm.Pixels[x * spiltWidth + y] = source.Pixels[start + x * w + y];
                        }
                    }
                    list.Add(bm);
                }
                
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        ImageSource = list;
                    });
            }
            catch (Exception e)
            {
                LogUtil.Log(e.StackTrace);
#if DEBUG
                throw;
#endif

            }
        }

        #endregion
        
    }
}
