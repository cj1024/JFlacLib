using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace CJToolkit.ImageExtention
{
    public class ImageService
    {

        #region 常量

        const double MinScale = 0.6;
        const double MaxScale = 2.0;
        private const int OpenDuration = 300;
        private const int CloseDuration = 300;
        private const int TransformDuration = 600;

        #endregion

        #region 私有成员

        private static Popup _instance;
        private static Storyboard _openAnime;
        private static Storyboard _closeAnime;
        private static Storyboard _resumeAnime;
        private static ImageWithDefault _image;
        private static CompositeTransform _transform;
        private static Boolean _locked;
        private static Boolean HasTransform
        {
            get
            {
                return _transform == null ||
                       !(_transform.ScaleX.Equals(1) && _transform.ScaleY.Equals(1) && _transform.TranslateX.Equals(0) &&
                         _transform.TranslateY.Equals(0));
            }
        }


        #endregion

        #region 默认初始化

        private static void Initial()
        {
            _locked = false;
            if(_transform==null)
            {
                _transform=new CompositeTransform();
            }
            if (_image == null)
            {
                _image = new ImageWithDefault
                             {
                                 HorizontalAlignment = HorizontalAlignment.Center,
                                 VerticalAlignment = VerticalAlignment.Center,
                                 Opacity = 0,
                                 Stretch = Stretch.Uniform,
                                 Width = 480,
                                 MinHeight = 200,
                                 FailImageSource = new BitmapImage{UriSource = new Uri("/icons/酒店详情暂无图片.png", UriKind.Relative),CreateOptions = BitmapCreateOptions.None}
                             };
                _image.DoubleTap += ImageDoubleTap;
                _image.RealImageFailed += ImageRealImageFailed;
                _image.RenderTransformOrigin = new Point(0.5, 0.5);
                _image.RenderTransform = _transform;
            }
            if (_instance == null)
            {
                _instance = new Popup {IsOpen = false};
                var layoutRoot = new Grid
                                     {
                                         Height = 800,
                                         Width = 480,
                                         Background = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x00, 0x00))
                                     };
                layoutRoot.Children.Add(_image);
                _instance.Child = layoutRoot;
                 ((PhoneApplicationFrame) Application.Current.RootVisual).Navigating+=ImageServiceNavigating;
            }
        }

        #endregion

        #region 对外调用方法

        public static void Start(Uri uri)
        {
            Initial();
            _image.Opacity = 0;
            _image.Source = uri;
            _transform.TranslateX = 0;
            _transform.TranslateY = 0;
            CreateOpenAnime();
            _openAnime.Begin();
            _instance.IsOpen = true;
        }
        
        public static void Stop()
        {
            if (_instance == null || _image == null) return;
            if (_closeAnime != null && _closeAnime.GetCurrentState() == ClockState.Active) return;
            CreateCloseAnime();
            if (_closeAnime != null) _closeAnime.Begin();
        }

        public static void Resume()
        {
            if (_instance == null || _image == null) return;
            CreateResumeAnime();
            _resumeAnime.Begin();
        }

        public static Boolean IsOpen
        {
            get { return _instance != null && _instance.IsOpen; }
        }

        #endregion

        #region 创建动画

        static void CreateOpenAnime()
        {
            _openAnime=new Storyboard();
            //透明度部分
            var da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(OpenDuration),
                             From = 0,
                             To = 1
                         };
            Storyboard.SetTarget(da,_image);
            Storyboard.SetTargetProperty(da,new PropertyPath("(UIElement.Opacity)"));
            _openAnime.Children.Add(da);
            
            _openAnime.Completed += OpenAnimeCompleted;
        }
        
        static void CreateCloseAnime()
        {
            _closeAnime = new Storyboard();
            //透明度部分
            var da = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(CloseDuration),
                From = 1,
                To = 0,
                BeginTime = TimeSpan.FromMilliseconds(HasTransform?TransformDuration:0)
            };
            Storyboard.SetTarget(da, _image);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            _closeAnime.Children.Add(da);
            if (HasTransform)
            {
                //恢复Translate
                da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(TransformDuration),
                             From = _transform.TranslateX,
                             To = 0,
                             EasingFunction = new QuarticEase {EasingMode = EasingMode.EaseOut}
                         };
                Storyboard.SetTarget(da, _transform);
                Storyboard.SetTargetProperty(da, new PropertyPath("TranslateX"));
                _closeAnime.Children.Add(da);
                da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(TransformDuration),
                             From = _transform.TranslateY,
                             To = 0,
                             EasingFunction = new QuarticEase {EasingMode = EasingMode.EaseOut}
                         };
                Storyboard.SetTarget(da, _transform);
                Storyboard.SetTargetProperty(da, new PropertyPath("TranslateY"));
                _closeAnime.Children.Add(da);
                //恢复Translate
                da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(TransformDuration),
                             From = _transform.ScaleX,
                             To = 1,
                             EasingFunction = new QuarticEase {EasingMode = EasingMode.EaseOut}
                         };
                Storyboard.SetTarget(da, _transform);
                Storyboard.SetTargetProperty(da, new PropertyPath("ScaleX"));
                _closeAnime.Children.Add(da);
                da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(TransformDuration),
                             From = _transform.ScaleY,
                             To = 1,
                             EasingFunction = new QuarticEase {EasingMode = EasingMode.EaseOut}
                         };
                Storyboard.SetTarget(da, _transform);
                Storyboard.SetTargetProperty(da, new PropertyPath("ScaleY"));
                _closeAnime.Children.Add(da);
            }
            _closeAnime.Completed += CloseAnimeCompleted;
        }

        static void CreateResumeAnime()
        {
            _resumeAnime=new Storyboard();
            if (!HasTransform) return;
            //恢复Translate
            var da = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(TransformDuration),
                From = _transform.TranslateX,
                To = 0,
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(da, _transform);
            Storyboard.SetTargetProperty(da, new PropertyPath("TranslateX"));
            _resumeAnime.Children.Add(da);
            da = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(TransformDuration),
                From = _transform.TranslateY,
                To = 0,
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(da, _transform);
            Storyboard.SetTargetProperty(da, new PropertyPath("TranslateY"));
            _resumeAnime.Children.Add(da);
            //恢复Translate
            da = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(TransformDuration),
                From = _transform.ScaleX,
                To = 1,
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(da, _transform);
            Storyboard.SetTargetProperty(da, new PropertyPath("ScaleX"));
            _resumeAnime.Children.Add(da);
            da = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(TransformDuration),
                From = _transform.ScaleY,
                To = 1,
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(da, _transform);
            Storyboard.SetTargetProperty(da, new PropertyPath("ScaleY"));
            _resumeAnime.Children.Add(da);
            _resumeAnime.Completed += ResumeAnimeCompleted;
        }

        #endregion

        #region 事件

        static void OpenAnimeCompleted(object sender, EventArgs e)
        {
            ((Storyboard)sender).Completed -= OpenAnimeCompleted;
            _image.Opacity = 1;
            //_image.MouseMove -= ImageMouseMove;
            //_image.MouseMove += ImageMouseMove;
            //_image.MouseEnter -= ImageMouseEnter;
            //_image.MouseEnter += ImageMouseEnter;
            _image.ManipulationDelta -= ImageManipulationDelta;
            _image.ManipulationDelta += ImageManipulationDelta;
            _instance.Child.ManipulationDelta -= GridManipulationDelta;
            _instance.Child.ManipulationDelta += GridManipulationDelta;
        }

        static void CloseAnimeCompleted(object sender, EventArgs e)
        {
            ((Storyboard)sender).Completed -= CloseAnimeCompleted;
            _instance.IsOpen = false;
            _image.Opacity = 0;
            _transform.TranslateX = 0;
            _transform.TranslateY = 0;
        }

        static void ResumeAnimeCompleted(object sender, EventArgs e)
        {
            ((Storyboard)sender).Completed -= ResumeAnimeCompleted;
            _transform.ScaleX = 1;
            _transform.ScaleY = 1;
            _transform.TranslateX = 0;
            _transform.TranslateY = 0;
        }

        static void ImageManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if(_locked) return;
            var translate = e.DeltaManipulation.Translation;
            _transform.TranslateX += translate.X * _transform.ScaleX;
            _transform.TranslateY += translate.Y * _transform.ScaleY;
        }

        static void GridManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (_locked) return;
            if (!e.DeltaManipulation.Scale.X.Equals(0) && !e.DeltaManipulation.Scale.Y.Equals(0))
            {
                var scale = e.DeltaManipulation.Scale;
                scale.X = (scale.X - 1)/1.5;
                scale.Y = (scale.Y - 1)/1.5;
                var delta = Math.Abs(scale.X) > Math.Abs(scale.Y) ? scale.X : scale.Y;
                if (delta > 0.2) delta = 0.2;
                if (_transform.ScaleX + delta <= MinScale)
                {
                    _transform.ScaleX = MinScale;
                    _transform.ScaleY = MinScale;
                }
                else if (_transform.ScaleX + delta >= MaxScale)
                {
                    _transform.ScaleX = MaxScale;
                    _transform.ScaleY = MaxScale;
                }
                else
                {
                    _transform.ScaleX += delta;
                    _transform.ScaleY += delta;
                }
            }
        }

        static void ImageDoubleTap(object sender, GestureEventArgs e)
        {
            if (!HasTransform)
                Stop();
            else
            {
                if (_resumeAnime != null && _resumeAnime.GetCurrentState() == ClockState.Active) return;
                Resume();
            }
        }

        static void ImageServiceNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_instance != null && _instance.IsOpen&&e.NavigationMode==NavigationMode.Back)
            {
                if(!e.IsCancelable) return;
                e.Cancel = true;
                if (_closeAnime != null && _closeAnime.GetCurrentState() == ClockState.Active) return;
                Stop();
            }
        }

        static void ImageRealImageFailed(object sender, EventArgs e)
        {
            //_image.Source = new Uri("/icons/酒店详情暂无图片.png", UriKind.Relative);
            _locked = true;
        }

        #endregion
    }
}
