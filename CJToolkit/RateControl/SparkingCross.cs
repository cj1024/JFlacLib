using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CJToolkit.RateControl
{
    [TemplatePart(Name = CrossPart,Type=typeof(Cross))]
    public class SparkingCross:Control
    {
        #region TemplatePart Name

        private const String CrossPart = "CrossPart";

        #endregion

        #region TemplatePart

        private Cross _crossPart;

        #endregion

        #region Constructor

        public SparkingCross()
        {
            DefaultStyleKey = typeof (SparkingCross);
            SparkingCrossService.InitializeItem(this);
        }

        ~SparkingCross()
        {
            SparkingCrossService.FinalizeItem(this);
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _crossPart = GetTemplateChild(CrossPart) as Cross;
            if (_crossPart != null)
                _crossPart.Foreground = new SolidColorBrush(DarkBrush);
        }

        #endregion

        #region 依赖属性

        public static readonly DependencyProperty LightBrushProperty =
            DependencyProperty.Register("LightBrush", typeof (Color), typeof (SparkingCross),
                                        new PropertyMetadata(Color.FromArgb(0xff,0xff,0xff,0xff)));

        public Color LightBrush
        {
            get { return (Color)GetValue(LightBrushProperty); }
            set { SetValue(LightBrushProperty, value); }
        }

        public static readonly DependencyProperty DarkBrushProperty =
            DependencyProperty.Register("DarkBrush", typeof(Color), typeof(SparkingCross),
                                        new PropertyMetadata(Color.FromArgb(0xff,0x35,0x35,0x35)));

        public Color DarkBrush
        {
            get { return (Color)GetValue(DarkBrushProperty); }
            set { SetValue(DarkBrushProperty, value); }
        }

        #endregion

        #region 私有方法

        internal void GenerateSparkingAnimeAndPlay()
        {
            if(_crossPart==null) return;
            var anime = new Storyboard {AutoReverse = true};
            var ca = new ColorAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(400),
                    From = DarkBrush,
                    To = LightBrush,
                    EasingFunction = new QuarticEase {EasingMode = EasingMode.EaseIn}
                };
            Storyboard.SetTarget(ca,_crossPart);
            Storyboard.SetTargetProperty(ca, new PropertyPath("(Cross.Foreground).(SoildColorBrush.Color)"));
            anime.Children.Add(ca);
            anime.Begin();
        }

        #endregion
        
    }

    internal class SparkingCrossService
    {
        private static List<WeakReference> _allItems;
        private static DispatcherTimer _timer;

        internal static bool IsEnabled { get { return _timer != null && _timer.IsEnabled; } }
        
        internal static void InitializeItem(SparkingCross item)
        {
            if(_allItems==null) _allItems=new List<WeakReference>();
            var referrence = new WeakReference(item);
            _allItems.Add(referrence);
            StartService();
        }

        internal static void FinalizeItem(SparkingCross item)
        {
            if (_allItems == null) return;
            for (int i = 0; i < _allItems.Count; i++)
            {
                if(_allItems[i].Target==item)
                {
                    _allItems.RemoveAt(i);
                    break;
                }
            }
        }

        internal static void StartService()
        {
            if (IsEnabled) return;
            if (_timer != null)
                _timer.Stop();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(new Random().Next(12)) };
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        internal static void StopService()
        {
            if (_timer == null || !_timer.IsEnabled) return;
            _timer.Stop();
            _timer.Tick -= TimerTick;
        }

        static void TimerTick(object sender, EventArgs e)
        {
            StopService();
            if (_allItems != null && _allItems.Count != 0)
            {
                var item = (SparkingCross) _allItems[new Random().Next(_allItems.Count)].Target;
                item.GenerateSparkingAnimeAndPlay();
            }
            StartService();
        }
    }
}
