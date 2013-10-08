using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Info;

namespace WP7FlacPlayer.Util
{
    public static class MemoryDiagnosticsUtil
    {
        static Popup _popup;
        static TextBlock _currentMemoryBlock;
        static TextBlock _appUsageMemoryBlock;
        static DispatcherTimer _timer;
        static bool _forceGc;

        public static void Start(bool forceGc)
        {
            _forceGc = forceGc;
            CreatePopup();
            CreateTimer();
            ShowPopup();
            StartTimer();
        }

        private static void ShowPopup()
        {
            _popup.IsOpen = true;
        }

        private static void StartTimer()
        {
            _timer.Start();
        }

        private static void CreateTimer()
        {
            if (_timer != null)
                return;

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(300)};
            _timer.Tick += TimerTick;
        }

        static void TimerTick(object sender, EventArgs e)
        {
            if (_forceGc)
                GC.Collect();

            long mem = DeviceStatus.ApplicationCurrentMemoryUsage / (1024 * 1024);
            _appUsageMemoryBlock.Text =
                                                     ((Double)DeviceStatus.ApplicationCurrentMemoryUsage /
                                                     DeviceStatus.ApplicationMemoryUsageLimit).ToString("F4");
            _currentMemoryBlock.Text = mem.ToString("F4");
        }

        private static void CreatePopup()
        {
            if (_popup != null)
                return;

            _popup = new Popup();
            double fontSize = (double)Application.Current.Resources["PhoneFontSizeSmall"] - 2;
            var foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
            var sp = new StackPanel { Orientation = Orientation.Horizontal, Background = (Brush)Application.Current.Resources["PhoneSemitransparentBrush"] };
            _currentMemoryBlock = new TextBlock { Text = "---", FontSize = fontSize, Foreground = foreground };
            _appUsageMemoryBlock = new TextBlock { Text = "---", FontSize = fontSize, Foreground = foreground };
            sp.Children.Add(new TextBlock { Text = "Mem(M): ", FontSize = fontSize, Foreground = foreground });
            sp.Children.Add(_currentMemoryBlock);
            sp.Children.Add(new TextBlock { Text = "---Rate: ", FontSize = fontSize, Foreground = foreground });
            sp.Children.Add(_appUsageMemoryBlock);
            sp.RenderTransform = new CompositeTransform { Rotation = 90, TranslateX = 480, TranslateY = 420, CenterX = 0, CenterY = 0 };
            _popup.Child = sp;
        }

        public static void Stop()
        {
            HidePopup();
            StopTimer();
        }

        private static void StopTimer()
        {
            _timer.Stop();
        }

        private static void HidePopup()
        {
            _popup.IsOpen = false;
        }
    }
}
