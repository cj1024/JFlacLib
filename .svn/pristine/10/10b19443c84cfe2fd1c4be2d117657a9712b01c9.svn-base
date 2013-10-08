using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CJToolkit.NotifyMessage
{
    /// <summary>
    /// 自制MessagePrompt，目前只支持显示字符串
    /// </summary>
    public class NotifyMessage
    {
        private static Dictionary<Storyboard, Popup> _instances;
        private const int DefaultLatency = 3500;
        
        /// <summary>
        /// 最普通的显示文字
        /// </summary>
        /// <param name="message">要显示的文字</param>
        /// <param name="wrap">文字是否自动换行</param>
        /// <param name="verticalOffset">消息显示的垂直位置</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="foreground">字体颜色</param>
        /// <param name="background">背景颜色</param>
        /// <param name="latency">持续时间</param>
        public static void Show(string message = "", bool wrap = false, double verticalOffset = 0, double fontSize = 22d, Brush foreground = null, Brush background = null, int latency = DefaultLatency)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => RealShow(message, wrap, verticalOffset, fontSize, foreground, background, latency));
        }

        static void RealShow(string message = "", bool wrap = false, double verticalOffset = 0, double fontSize = 22d, Brush foreground = null, Brush background = null, int latency = DefaultLatency)
        {
            if (_instances == null)
                _instances = new Dictionary<Storyboard, Popup>();
            var layoutRoot = new Grid
            {
                Width = 480,
                Background = background ?? new SolidColorBrush(Color.FromArgb(0x99, 0x22, 0xbb, 0xff))
            };
            var messageBox = new TextBlock
            {
                Text = message,
                TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
                Foreground = foreground ?? new SolidColorBrush(Colors.White),
                FontSize = fontSize,
                Margin = new Thickness(12, 32, 12, 6)
            };
            layoutRoot.Children.Add(messageBox);
            Show(layoutRoot, verticalOffset, latency);
        }

        internal static void Show(UIElement message, double verticalOffset = 0, int latency = DefaultLatency)
        {
            var p = new Popup {VerticalOffset = verticalOffset, Child = message};
            PlayAnime(p, message,latency);
        }

        private static void PlayAnime(Popup popup, UIElement obj, int latency = DefaultLatency)
        {
            obj.CacheMode=new BitmapCache();
            obj.Opacity = 0;
            obj.RenderTransform=new CompositeTransform{CenterX = 0.5,CenterY = 0.5};
            var anime = new Storyboard();
            var da = new DoubleAnimation
                         {
                             BeginTime = TimeSpan.FromMilliseconds(latency),
                             Duration = TimeSpan.FromMilliseconds(500),
                             From = 0,
                             To = 480,
                             EasingFunction = new QuinticEase {EasingMode = EasingMode.EaseIn}
                         };
            Storyboard.SetTarget(da,obj);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            anime.Children.Add(da);
            da = new DoubleAnimation
                     {
                         BeginTime = TimeSpan.FromMilliseconds(300),
                         Duration = TimeSpan.FromMilliseconds(1200),
                         From = -32,
                         To = 0,
                         EasingFunction = new QuinticEase {EasingMode = EasingMode.EaseOut}
                     };
            Storyboard.SetTarget(da, obj);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            anime.Children.Add(da);
            da = new DoubleAnimation
            {
                BeginTime = TimeSpan.FromMilliseconds(300),
                Duration = TimeSpan.FromMilliseconds(500),
                From = 0,
                To = 1
            };
            Storyboard.SetTarget(da, obj);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            anime.Children.Add(da);
            if(_instances.Count>0)
            {
                foreach (var key in _instances.Keys)
                {
                    key.SkipToFill();
                }
            }
            _instances.Add(anime, popup);
            anime.Completed += (sender, e) =>
                                   {
                                       popup.IsOpen = false;
                                       _instances.Remove(anime);
                                   };
            anime.Begin();
            popup.IsOpen = true;
            obj.Tap += (sender, e) => ((Storyboard) sender).Seek(TimeSpan.FromMilliseconds(latency));
        }


    }
}
