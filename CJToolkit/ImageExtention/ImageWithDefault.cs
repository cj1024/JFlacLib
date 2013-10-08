using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CJToolkit.ImageExtention
{
    public class ImageWithDefault:Grid
    {
        private const string DefaultImage = "/ContentImages/bg_defaultpic.png";//默认底图
        private const string FailImage = "/icons/暂无图片.png";//加载失败后显示的图片

        private Storyboard _storyBoard;

        public ImageWithDefault()
        {
            ChangeSource();
            ChangeStretch();
        }

        #region 依赖属性

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(ImageWithDefault), new PropertyMetadata(null, SourceChanged));

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        static void SourceChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            ((ImageWithDefault)obj).ChangeSource();
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof (Stretch), typeof (ImageWithDefault), new PropertyMetadata(default(Stretch),StretchChanged));

        public Stretch Stretch
        {
            get { return (Stretch) GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        static void StretchChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            ((ImageWithDefault)obj).ChangeStretch();
        }

        public static readonly DependencyProperty FailImageSourceProperty =
            DependencyProperty.Register("FailImageSource", typeof(BitmapImage), typeof(ImageWithDefault), new PropertyMetadata(new BitmapImage { UriSource = new Uri(FailImage) }));

        public BitmapImage FailImageSource
        {
            get { return (BitmapImage) GetValue(FailImageSourceProperty); }
            set { SetValue(FailImageSourceProperty, value); }
        }   

        #endregion

        #region 私有方法

        void ChangeSource()
        {
            AddImage();
            Background = new ImageBrush {ImageSource = new BitmapImage(new Uri(DefaultImage, UriKind.Relative))};
            if (Source == null) return;
            var bitmapFromUri = new BitmapImage();
            bitmapFromUri.ImageOpened += ImageOpened;
            bitmapFromUri.ImageFailed += ImageFailed;
            bitmapFromUri.UriSource = Source;
            ((Image)Children[0]).Source = bitmapFromUri;
        }

        void ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ((BitmapImage)sender).ImageFailed -= ImageFailed;
            Background = new ImageBrush { ImageSource = FailImageSource,Opacity = 1,AlignmentX = AlignmentX.Center,AlignmentY = AlignmentY.Center,Stretch = Stretch.None};
            if (RealImageFailed != null)
                RealImageFailed(this, e);
        }

        void ImageOpened(object sender, RoutedEventArgs e)
        {
            ((BitmapImage)sender).ImageOpened -= ImageOpened;
            if (Children.Count > 0)
            {
                ((Image)Children[0]).Source = sender as BitmapImage;
            }
            else
            {
                Children.Add(new Image
                                 {Source = sender as BitmapImage, Stretch = Stretch, Width = Width, Height = Height});
            }
            Background = new SolidColorBrush(Colors.Transparent);
            PlayAnime(Children[0]);
        }

        void ChangeStretch()
        {
            if (Children != null && Children.Count > 0)
            {
                ((Image)Children[0]).Stretch = Stretch;
            }
        }

        void AddImage()
        {
            if (Children.Count== 0)
            {
                Children.Add(new Image {Width = Width, Height = Height, Stretch = Stretch});
            }
            Children[0].Opacity = 0;
        }

        void PlayAnime(DependencyObject obj)
        {
            if(_storyBoard!=null && _storyBoard.GetCurrentState()== ClockState.Active)
                _storyBoard.SkipToFill();
            _storyBoard=new Storyboard();
            var da = new DoubleAnimation
                         {
                             BeginTime = TimeSpan.FromMilliseconds(0),
                             Duration = TimeSpan.FromMilliseconds(1000),
                             From = 0,
                             To = 1
                         };
            Storyboard.SetTarget(da, obj);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            _storyBoard.Children.Add(da);
            _storyBoard.Completed += StoryBoardCompleted;
            _storyBoard.Begin();
        }

        void StoryBoardCompleted(object sender, EventArgs e)
        {
            Children[0].Opacity = 1;
        }

        #endregion

        #region 事件

        public delegate void ImageFailedHandler(object sender, EventArgs e);

        public event ImageFailedHandler RealImageFailed;

        #endregion

    }
}
