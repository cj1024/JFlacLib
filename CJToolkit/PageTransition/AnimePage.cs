using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Practices.Prism.Commands;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace CJToolkit.PageTransition
{
    public class AnimePage : PhoneApplicationPage
    {
        public enum AnimeType
        {
            /// <summary>
            /// 普通翻页的动画
            /// </summary>
            Flip,
            /// <summary>
            /// 转入转出左上角的动画
            /// </summary>
            Roll,
            /// <summary>
            /// 左右滑动的动画
            /// </summary>
            Slide,
            /// <summary>
            /// 单纯缩放的动画
            /// </summary>
            Stretch,
            /// <summary>
            /// 弹出的动画
            /// </summary>
            Popup
        }

        private Storyboard _naviAnime;

        private Point _touchPoint;

        private ICommand _animeCompletedCommand;

        public static readonly DependencyProperty ForwardInAnimeOnProperty =
            DependencyProperty.Register("ForwardInAnimeOn", typeof (bool), typeof (AnimePage), new PropertyMetadata(true));

        public bool ForwardInAnimeOn
        {
            get { return (bool) GetValue(ForwardInAnimeOnProperty); }
            set { SetValue(ForwardInAnimeOnProperty, value); }
        }

        public static readonly DependencyProperty ForwardOutAnimeOnProperty =
            DependencyProperty.Register("ForwardOutAnimeOn", typeof (bool), typeof (AnimePage), new PropertyMetadata(true));

        public bool ForwardOutAnimeOn
        {
            get { return (bool) GetValue(ForwardOutAnimeOnProperty); }
            set { SetValue(ForwardOutAnimeOnProperty, value); }
        }

        public static readonly DependencyProperty BackwardInAnimeOnProperty =
            DependencyProperty.Register("BackwardInAnimeOn", typeof(bool), typeof(AnimePage), new PropertyMetadata(true));

        public bool BackwardInAnimeOn
        {
            get { return (bool)GetValue(BackwardInAnimeOnProperty); }
            set { SetValue(BackwardInAnimeOnProperty, value); }
        }

        public static readonly DependencyProperty BackwardOutAnimeOnProperty =
            DependencyProperty.Register("BackwardOutAnimeOn", typeof(bool), typeof(AnimePage), new PropertyMetadata(true));

        public bool BackwardOutAnimeOn
        {
            get { return (bool)GetValue(BackwardOutAnimeOnProperty); }
            set { SetValue(BackwardOutAnimeOnProperty, value); }
        }

        public static readonly DependencyProperty ForwardInAnimeTypeProperty =
            DependencyProperty.Register("ForwardInAnimeType", typeof(AnimeType), typeof(AnimePage), new PropertyMetadata(AnimeType.Flip));

        public AnimeType ForwardInAnimeType
        {
            get { return (AnimeType)GetValue(ForwardInAnimeTypeProperty); }
            set { SetValue(ForwardInAnimeTypeProperty, value); }
        }

        public static readonly DependencyProperty BackwardInAnimeTypeProperty =
            DependencyProperty.Register("BackwardInAnimeType", typeof (AnimeType), typeof (AnimePage), new PropertyMetadata(AnimeType.Flip));

        public AnimeType BackwardInAnimeType
        {
            get { return (AnimeType) GetValue(BackwardInAnimeTypeProperty); }
            set { SetValue(BackwardInAnimeTypeProperty, value); }
        }

        /// <summary>
        /// 前进动画的大致时间
        /// </summary>
        private const int ForwardAnimeDuration = 800;

        /// <summary>
        /// 后退动画的大致时间
        /// </summary>
        private const int BackwardAnimeDuration = 800;

        /// <summary>
        /// 变形发生的大致时间
        /// </summary>
        private const int SkewDuration = 50;

        /// <summary>
        /// 动画最终透明度
        /// </summary>
        private const double FadeOpacity = 0.2;

        /// <summary>
        /// 旋转动画的最终size比例
        /// </summary>
        private const double RollMinScale = 0.1;

        /// <summary>
        /// 旋转动画的转动角度
        /// </summary>
        private const double RollAngle = -70;

        public AnimePage()
        {
            CacheMode = new BitmapCache();
            Tap += AnimePageTap;
        }

        void AnimePageTap(object sender, GestureEventArgs e)
        {
            _touchPoint = e.GetPosition(this);
        }

        private void NaviAnimeCompleted(object sender, EventArgs e)
        {
            var grid = FindName("LayoutRoot") as Grid;
            if (grid != null&&grid.Children.Contains(App.Element)) grid.Children.Remove(App.Element);
            App.Element = new Grid();
            var storyboard = sender as Storyboard;
            if (storyboard != null) storyboard.Completed -= NaviAnimeCompleted;
            if (_animeCompletedCommand != null) _animeCompletedCommand.Execute(null);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if ((e.NavigationMode==NavigationMode.New && ForwardOutAnimeOn)||(e.NavigationMode==NavigationMode.Back&&BackwardOutAnimeOn))
            {
                //截图并创建下一个页面要用的Grid
                if (_naviAnime != null && _naviAnime.GetCurrentState() == ClockState.Active)
                    _naviAnime.SkipToFill();
                var layout = FindName("LayoutRoot") as Grid;
                if (layout != null)
                {
                    layout.Children.Remove(App.Element);
                    App.Element = new Grid();
                    try
                    {
                        var bmp = new WriteableBitmap(layout, new CompositeTransform());
                        bmp.Invalidate();
                        var grid = new Grid();
                        var imageBrush = new ImageBrush {ImageSource = bmp, Stretch = Stretch.None};
                        grid.Background = imageBrush;
                        grid.Width = bmp.PixelWidth;
                        grid.Height = bmp.PixelHeight;
                        grid.VerticalAlignment = VerticalAlignment.Top;
                        App.Element = grid;
                    }
                    catch (Exception exception)
                    {
                        NotifyMessage.NotifyMessage.Show(exception.StackTrace);
#if DEBUG
                        throw;
#endif
                    }
                }
            }
            else
            {
                App.Element = new Grid();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)//forwardin
            {
                Loaded += AnimePageLoaded;
                if (App.Element != null)
                {
                    var grid = FindName("LayoutRoot") as Grid;
                    if (grid != null)
                    {
                        grid.Children.Add(App.Element);
                        switch (ForwardInAnimeType)
                        {
                            case AnimeType.Flip:
                                CreateNaviAnime(App.Element, ForwardInAnimeOn ? grid.Children[0] : new Grid(), true);
                                //_naviAnime = StoryBoardFactory.CreateFlipStoryboard(App.Element, ForwardInAnimeOn ? grid.Children[0] : new Grid(),FlipMode.FlipForward);
                                break;
                            case AnimeType.Roll:
                                CreateNaviAnime1(App.Element, ForwardInAnimeOn ? grid.Children[0] : new Grid(), true);
                                break;
                            case AnimeType.Slide:
                                CreateNaviAnime2(App.Element, ForwardInAnimeOn ? grid.Children[0] : new Grid(), true);
                                break;
                            case AnimeType.Stretch:
                                CreateNaviAnime3(App.Element, ForwardInAnimeOn ? grid.Children[0] : new Grid(), true);
                                break;
                            case AnimeType.Popup:
                                CreateNaviAnime4(App.Element, ForwardInAnimeOn ? grid.Children[0] : new Grid(), true);
                                break;
                        }
                    }
                }

            }
            else//backward
            {
                if (App.Element != null)
                {
                    var grid = FindName("LayoutRoot") as Grid;
                    if(!BackwardInAnimeOn) return;
                    if (grid != null)
                    {
                        grid.Children.Add(App.Element);
                        switch (BackwardInAnimeType)
                        {
                            case AnimeType.Flip:
                                //_naviAnime = StoryBoardFactory.CreateFlipStoryboard(App.Element, BackwardInAnimeOn ? grid.Children[0] : new Grid(), FlipMode.FlipBackward);
                                CreateNaviAnime(BackwardInAnimeOn ? grid.Children[0] : new Grid(), App.Element, false);
                                break;
                            case AnimeType.Roll:
                                CreateNaviAnime1(BackwardInAnimeOn ? grid.Children[0] : new Grid(), App.Element, false);
                                break;
                            case AnimeType.Slide:
                                CreateNaviAnime2(BackwardInAnimeOn ? grid.Children[0] : new Grid(), App.Element, false);
                                break;
                            case AnimeType.Stretch:
                                CreateNaviAnime3(BackwardInAnimeOn ? grid.Children[0] : new Grid(), App.Element, false);
                                break;
                            case AnimeType.Popup:
                                CreateNaviAnime4(BackwardInAnimeOn ? grid.Children[0] : new Grid(), App.Element, false);
                                break;
                        }
                        try
                        {
                            _naviAnime.Begin();
                        }
                        catch (Exception exception)
                        {
                            NotifyMessage.NotifyMessage.Show(exception.StackTrace);
#if DEBUG
                            throw;
#endif
                        }
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.Content is AnimePage && e.NavigationMode == NavigationMode.New)
            {
                (e.Content as AnimePage)._touchPoint = _touchPoint;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if(_naviAnime!=null&&_naviAnime.GetCurrentState()==ClockState.Active)
                _naviAnime.SkipToFill();
        }

        private void AnimePageLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= AnimePageLoaded;
            if (_naviAnime != null)
            {
                try
                {
                    _naviAnime.Begin();
                }
                catch (Exception exception)
                {
                    NotifyMessage.NotifyMessage.Show(exception.StackTrace);
#if DEBUG
                    throw;
#endif
                }
            }
        }

        /// <summary>
        /// 创建旋转并伴有透明度变化的动画
        /// </summary>
        /// <param name="element1">动画1的对象</param>
        /// <param name="element2">动画2的对象</param>
        /// <param name="isNew">true代表是新建的页面，false代表是回退页面</param>
        private void CreateNaviAnime(UIElement element1, UIElement element2, bool isNew)
        {
            _naviAnime = new Storyboard();
            _naviAnime.Completed += NaviAnimeCompleted;
            var pp = new PlaneProjection {CenterOfRotationX = 0};
            element1.Projection = pp;
            //翻转部分
            var da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                             BeginTime = TimeSpan.FromMilliseconds(0),
                             From = isNew ? 0 : 90,
                             To = isNew ? 90 : 0
                         };
            Storyboard.SetTarget(da, element1);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
            _naviAnime.Children.Add(da);
            //透明部分
            da = new DoubleAnimation
                     {
                         Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                         BeginTime = TimeSpan.FromMilliseconds(0),
                         From = isNew ? 1 : FadeOpacity,
                         To = isNew ? FadeOpacity : 1
                     };
            Storyboard.SetTarget(da, element1);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            _naviAnime.Children.Add(da);
            pp = new PlaneProjection {CenterOfRotationX = 0};
            element2.Projection = pp;
            //翻转部分
            da = new DoubleAnimation
                     {
                         Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                         BeginTime = TimeSpan.FromMilliseconds(0),
                         From = isNew ? -90 : 0,
                         To = isNew ? 0 : -90
                     };
            Storyboard.SetTarget(da, element2);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
            _naviAnime.Children.Add(da);
            //透明部分
            da = new DoubleAnimation
                     {
                         Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                         BeginTime = TimeSpan.FromMilliseconds(0),
                         From = isNew ? FadeOpacity : 1,
                         To = isNew ? 1 : FadeOpacity
                     };
            Storyboard.SetTarget(da, element2);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            _naviAnime.Children.Add(da);
        }

        /// <summary>
        /// 创建转出转入的动画，并伴有透明度
        /// </summary>
        /// <param name="element1">动画1的对象</param>
        /// <param name="element2">动画2的对象</param>
        /// <param name="isNew">true代表是新建的页面，false代表是回退页面</param>
        private void CreateNaviAnime1(UIElement element1, UIElement element2, bool isNew)
        {
            _naviAnime = new Storyboard();
            _naviAnime.Completed += NaviAnimeCompleted;
            var rt = new CompositeTransform {CenterX = 0, CenterY = 0};
            element1.RenderTransform = rt;
            //翻转部分
            var da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                             BeginTime = TimeSpan.FromMilliseconds(0),
                             From = isNew ? 0 : RollAngle,
                             To = isNew ? RollAngle : 0
                         };
            Storyboard.SetTarget(da, element1);
            Storyboard.SetTargetProperty(da,
                                         new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)"));
            _naviAnime.Children.Add(da);
            //透明部分
            da = new DoubleAnimation
                     {
                         Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                         BeginTime = TimeSpan.FromMilliseconds(0),
                         From = isNew ? 1 : FadeOpacity,
                         To = isNew ? FadeOpacity : 1
                     };
            Storyboard.SetTarget(da, element1);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            _naviAnime.Children.Add(da);
            //尺寸部分
            rt = new CompositeTransform {CenterX = 0.5, CenterY = 0.5};
            element1.RenderTransform = rt;
            da = new DoubleAnimation
                     {
                         Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                         BeginTime = TimeSpan.FromMilliseconds(0),
                         From = isNew ? 1 : RollMinScale,
                         To = isNew ? RollMinScale : 1
                     };
            Storyboard.SetTarget(da, element1);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
            _naviAnime.Children.Add(da);
            da = new DoubleAnimation
                     {
                         Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                         BeginTime = TimeSpan.FromMilliseconds(0),
                         From = isNew ? 1 : RollMinScale,
                         To = isNew ? RollMinScale : 1
                     };
            Storyboard.SetTarget(da, element1);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
            _naviAnime.Children.Add(da);
            //翻转部分
            //da = new DoubleAnimation
            //         {
            //             Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
            //             BeginTime = TimeSpan.FromMilliseconds(0),
            //             From = isNew ? -90 : 0,
            //             To = isNew ? 0 : -90
            //         };
            //Storyboard.SetTarget(da, element2);
            //Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
            //_naviAnime.Children.Add(da);
            //透明部分
            da = new DoubleAnimation
                     {
                         Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
                         BeginTime = TimeSpan.FromMilliseconds(0),
                         From = isNew ? FadeOpacity : 1,
                         To = isNew ? 1 : FadeOpacity
                     };
            Storyboard.SetTarget(da, element2);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            _naviAnime.Children.Add(da);
            _naviAnime.BeginTime = TimeSpan.FromMilliseconds(isNew?300:0);
        }

        /// <summary>
        /// 创建左右滑动并伴有透明度变化的动画
        /// </summary>
        /// <param name="element1">动画1的对象</param>
        /// <param name="element2">动画2的对象</param>
        /// <param name="isNew">true代表是新建的页面，false代表是回退页面</param>
        private void CreateNaviAnime2(UIElement element1, UIElement element2, bool isNew)
        {
            _naviAnime = new Storyboard();
            _naviAnime.Completed += NaviAnimeCompleted;
            if (isNew)
            {
                var rt = new CompositeTransform {CenterX = 1, CenterY = 0};
                element1.RenderTransform = rt;
                //element1变形部分
                var da = new DoubleAnimationUsingKeyFrames();
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                                         KeyTime = TimeSpan.FromMilliseconds(10),
                                         Value = 0
                                     });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration),
                    Value = -15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration+ForwardAnimeDuration),
                    Value = -15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration+SkewDuration + ForwardAnimeDuration),
                    Value = 0
                });
                Storyboard.SetTarget(da, element1);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.SkewX)"));
                _naviAnime.Children.Add(da);
                //element1移动部分
                var da1 = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(ForwardAnimeDuration),
                             BeginTime = TimeSpan.FromMilliseconds(SkewDuration),
                             From = 0,
                             To = -480
                         };
                Storyboard.SetTarget(da1, element1);
                Storyboard.SetTargetProperty(da1,
                                             new PropertyPath(
                                                 "(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                _naviAnime.Children.Add(da1);
                //element2变形部分1
                rt = new CompositeTransform { CenterX = 1, CenterY = 0};
                element2.RenderTransform = rt;
                da = new DoubleAnimationUsingKeyFrames();
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(10),
                    Value = 0
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration),
                    Value = -15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration + ForwardAnimeDuration),
                    Value = -15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration + SkewDuration + ForwardAnimeDuration),
                    Value = 0
                });
                Storyboard.SetTarget(da, element2);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.SkewX)"));
                _naviAnime.Children.Add(da);
                //element2移动部分
                da1 = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(ForwardAnimeDuration),
                             BeginTime = TimeSpan.FromMilliseconds(SkewDuration),
                             From = 480,
                             To = 0
                         };
                Storyboard.SetTarget(da1, element2);
                Storyboard.SetTargetProperty(da1,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                _naviAnime.Children.Add(da1);
            }
            else
            {
                var rt = new CompositeTransform { CenterX = 0, CenterY = 0 };
                element2.RenderTransform = rt;
                //element2变形部分
                var da = new DoubleAnimationUsingKeyFrames();
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(10),
                    Value = 0
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration),
                    Value = 15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration + ForwardAnimeDuration),
                    Value = 15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration + SkewDuration + ForwardAnimeDuration),
                    Value = 0
                });
                Storyboard.SetTarget(da, element2);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.SkewX)"));
                _naviAnime.Children.Add(da);
                //element2移动部分
                var da1 = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(BackwardAnimeDuration),
                    BeginTime = TimeSpan.FromMilliseconds(SkewDuration),
                    From = 0,
                    To = 480
                };
                Storyboard.SetTarget(da1, element2);
                Storyboard.SetTargetProperty(da1,
                                             new PropertyPath(
                                                 "(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                _naviAnime.Children.Add(da1);
                //element1变形部分
                rt = new CompositeTransform { CenterX = 1, CenterY = 1 };
                element1.RenderTransform = rt;
                da = new DoubleAnimationUsingKeyFrames();
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(0),
                    Value = 0
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration),
                    Value = 15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration + ForwardAnimeDuration),
                    Value = 15
                });
                da.KeyFrames.Add(new EasingDoubleKeyFrame
                                     {
                    KeyTime = TimeSpan.FromMilliseconds(SkewDuration + SkewDuration + ForwardAnimeDuration),
                    Value = 0
                });
                Storyboard.SetTarget(da, element1);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.SkewX)"));
                _naviAnime.Children.Add(da);
                //element1移动部分
                da1 = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(BackwardAnimeDuration),
                    BeginTime = TimeSpan.FromMilliseconds(SkewDuration),
                    From = -480,
                    To = 0
                };
                Storyboard.SetTarget(da1, element1);
                Storyboard.SetTargetProperty(da1,
                                             new PropertyPath(
                                                 "(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                _naviAnime.Children.Add(da1);
            }
            //翻转部分
            //da = new DoubleAnimation
            //         {
            //             Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
            //             BeginTime = TimeSpan.FromMilliseconds(0),
            //             From = isNew ? -90 : 0,
            //             To = isNew ? 0 : -90
            //         };
            //Storyboard.SetTarget(da, element2);
            //Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
            //_naviAnime.Children.Add(da);
            _naviAnime.BeginTime = TimeSpan.FromMilliseconds(isNew?300:0);
        }

        /// <summary>
        /// 创建缩放的动画
        /// </summary>
        /// <param name="element1">动画1的对象</param>
        /// <param name="element2">动画2的对象</param>
        /// <param name="isNew">true代表是新建的页面，false代表是回退页面</param>
        private void CreateNaviAnime3(UIElement element1, UIElement element2, bool isNew)
        {
            _naviAnime = new Storyboard();
            _naviAnime.Completed += NaviAnimeCompleted;
            if (isNew)
            {
                Canvas.SetZIndex(element1, -1);
                var rt = new ScaleTransform { CenterX = 240, CenterY = _touchPoint.Y ,ScaleX = 0,ScaleY = 0};
                element2.RenderTransform = rt;
                _animeCompletedCommand = new DelegateCommand(() =>
                                                                 {
                                                                     element2.RenderTransform = new ScaleTransform
                                                                                                    {
                                                                                                        ScaleX = 1,
                                                                                                        ScaleY = 1
                                                                                                    };
                                                                     Canvas.SetZIndex(element1, 0);
                                                                 });
                //element2变形部分
                var da = new DoubleAnimation
                             {
                                 Duration = TimeSpan.FromMilliseconds(ForwardAnimeDuration),
                                 BeginTime = TimeSpan.FromMilliseconds(0),
                                 From = 0,
                                 To = 1
                             };
                Storyboard.SetTarget(da, element2);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
                _naviAnime.Children.Add(da);
                da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(ForwardAnimeDuration),
                             BeginTime = TimeSpan.FromMilliseconds(0),
                             From = 0,
                             To = 1
                         };
                Storyboard.SetTarget(da, element2);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
                _naviAnime.Children.Add(da);
            }
            else
            {
                var rt = new ScaleTransform
                             {CenterX = 240, CenterY = _touchPoint.Y};
                element2.RenderTransform = rt;
                //element2变形部分
                var da = new DoubleAnimation
                             {
                                 Duration = TimeSpan.FromMilliseconds(BackwardAnimeDuration),
                                 BeginTime = TimeSpan.FromMilliseconds(0),
                                 From = 1,
                                 To = 0
                             };
                Storyboard.SetTarget(da, element2);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
                _naviAnime.Children.Add(da);
                da = new DoubleAnimation
                         {
                             Duration = TimeSpan.FromMilliseconds(BackwardAnimeDuration),
                             BeginTime = TimeSpan.FromMilliseconds(0),
                             From = 1,
                             To = 0
                         };
                Storyboard.SetTarget(da, element2);
                Storyboard.SetTargetProperty(da,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
                _naviAnime.Children.Add(da);
            }
        }

        /// <summary>
        /// 创建弹出的动画
        /// </summary>
        /// <param name="element1">动画1的对象</param>
        /// <param name="element2">动画2的对象</param>
        /// <param name="isNew">true代表是新建的页面，false代表是回退页面</param>
        private void CreateNaviAnime4(UIElement element1, UIElement element2, bool isNew)
        {
            _naviAnime = new Storyboard();
            _naviAnime.Completed += NaviAnimeCompleted;
            var rt = new CompositeTransform {CenterX = 0, CenterY = 0};
            element2.RenderTransform = rt;
            Canvas.SetZIndex(element2, 1);
            (element2.RenderTransform as CompositeTransform).TranslateY = 800;
            _animeCompletedCommand = new DelegateCommand(() =>
                                                             {
                                                                 var compositeTransform =
                                                                     element2.RenderTransform as CompositeTransform;
                                                                 if (compositeTransform !=
                                                                     null)
                                                                     compositeTransform.TranslateY = 0;
                                                                 Canvas.SetZIndex(element2, 0);
                                                             });
            //位移部分
            IEasingFunction easingFunction;
            if(isNew)
                easingFunction= new BackEase {EasingMode = EasingMode.EaseOut, Amplitude = 0.2};
            else
                easingFunction = new CubicEase {EasingMode = EasingMode.EaseIn};
            var da = new DoubleAnimation
                         {
                             Duration =
                                 TimeSpan.FromMilliseconds(ForwardAnimeDuration),
                             BeginTime = TimeSpan.FromMilliseconds(0),
                             From = isNew ? 800 : 0,
                             To = isNew ? 0 : 800,
                             EasingFunction = easingFunction
                         };
            Storyboard.SetTarget(da, element2);
            Storyboard.SetTargetProperty(da,
                                            new PropertyPath(
                                                "(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            _naviAnime.Children.Add(da);
            //透明部分
            da = new DoubleAnimation
                        {
                            Duration = TimeSpan.FromMilliseconds(ForwardAnimeDuration),
                            BeginTime = TimeSpan.FromMilliseconds(0),
                            From = isNew ? 1 : FadeOpacity,
                            To = isNew ? FadeOpacity : 1
                        };
            Storyboard.SetTarget(da, element1);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            _naviAnime.Children.Add(da);
            //位移部分
            //da = new DoubleAnimation
            //         {
            //             Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
            //             BeginTime = TimeSpan.FromMilliseconds(0),
            //             From = isNew ? -90 : 0,
            //             To = isNew ? 0 : -90
            //         };
            //Storyboard.SetTarget(da, element2);
            //Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
            //_naviAnime.Children.Add(da);
            //透明部分
            //da = new DoubleAnimation
            //{
            //    Duration = TimeSpan.FromMilliseconds(isNew ? ForwardAnimeDuration : BackwardAnimeDuration),
            //    BeginTime = TimeSpan.FromMilliseconds(0),
            //    From = isNew ? FadeOpacity : 1,
            //    To = isNew ? 1 : FadeOpacity
            //};
            //Storyboard.SetTarget(da, element1);
            //Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            //_naviAnime.Children.Add(da);
            _naviAnime.BeginTime = TimeSpan.FromMilliseconds(isNew ? 100 : 0);
        }

    }

    public class App
    {
        public static Grid Element { get; set; }
    }
}
