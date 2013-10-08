using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CJToolkit.ImageExtention
{
    [TemplatePart(Name = Image, Type = typeof(ImageBrush))]
    [TemplatePart(Name = ViewPortTransform, Type = typeof(TranslateTransform))]
    public class ImageHubTile:Control
    {
        #region TemlatePart Name

        private const String Image = "Image";
        private const String ViewPortTransform = "ViewPortTransform";

        #endregion

        #region TemplatePart

        private ImageBrush _image;
        private TranslateTransform _viewPortTransform;
        
        #endregion

        #region 属性

        public double ImageWidth { get; private set; }

        public double ImageHeight { get; private set; }

        public Orientation AnimationOrientation { get; private set; }

        public double MaxiumTranslate { get; private set; }

        private double _current;

        private Storyboard _translateAnimation;

        #endregion

        #region 常量

        private const int AnimationGap = 1000;
        private const int AimationDuration = 3000;

        #endregion

        #region 构造函数

        public ImageHubTile()
        {
            DefaultStyleKey = typeof (ImageHubTile);
        }

        #endregion

        #region 依赖属性

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof (BitmapImage), typeof (ImageHubTile), new PropertyMetadata(null,OnImageSourceChanged));

        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }   

        static void OnImageSourceChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            ((ImageHubTile) obj).ImageSourceChanged();
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _image = GetTemplateChild(Image) as ImageBrush;
            _viewPortTransform = GetTemplateChild(ViewPortTransform) as TranslateTransform;
            ImageSourceChanged();
            Start();
        }

        #endregion

        #region 私有方法

        void ImageSourceChanged()
        {
            if(ImageSource==null||_image==null) return;
            _image.ImageSource = ImageSource;
            ImageSource.ImageOpened += (sender, e) => ChangeAnimationType();
            ChangeAnimationType();
        }

        void ChangeAnimationType()
        {
            ImageHeight = ImageSource.PixelHeight;
            ImageWidth = ImageSource.PixelWidth; 
            _current = 0;
            _viewPortTransform.X = 0;
            _viewPortTransform.Y = 0;
            if (ImageHeight < 1 || ImageHeight < 1 || Width < 1 || Height < 0)
            {
                AnimationOrientation = Orientation.Vertical;
                MaxiumTranslate = 0;
                return;
            }
            var imageHWRate = ImageHeight/ImageWidth;
            var viewportHWRate = Height/Width;
            AnimationOrientation = imageHWRate >= viewportHWRate ? Orientation.Vertical : Orientation.Horizontal;
            MaxiumTranslate = imageHWRate > viewportHWRate ? (Width * imageHWRate - Height) / Height : (Height / imageHWRate - Width) / Width;
        }

        void Start()
        {
            _translateAnimation = new Storyboard();
            _translateAnimation.Completed += TranslateAnimationCompleted;
            var da = new DoubleAnimation
                {
                    BeginTime = TimeSpan.FromMilliseconds(0),
                    Duration = TimeSpan.FromMilliseconds(AimationDuration),
                    From = _current,
                    To = Math.Abs(_current - 0) <= 0 ? (AnimationOrientation == Orientation.Vertical ? MaxiumTranslate : 0 - MaxiumTranslate) : 0,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
            Storyboard.SetTarget(da,_viewPortTransform);
            Storyboard.SetTargetProperty(da, new PropertyPath(AnimationOrientation == Orientation.Vertical ? "Y" : "X"));
            _translateAnimation.Children.Add(da);
            _translateAnimation.Begin();
        }


        #endregion

        #region 事件

        void TranslateAnimationCompleted(object sender, EventArgs e)
        {
            var t = new Thread(() =>
                {
                    _current = Math.Abs(_current - 0) <= 0 ? (AnimationOrientation == Orientation.Vertical ? MaxiumTranslate : 0 - MaxiumTranslate) : 0;
                    Thread.Sleep(Convert.ToInt32(AnimationGap*(new Random().NextDouble() + 0.5)));
                    Deployment.Current.Dispatcher.BeginInvoke(Start);
                });
            t.Start();
        }

        #endregion

    }

}
