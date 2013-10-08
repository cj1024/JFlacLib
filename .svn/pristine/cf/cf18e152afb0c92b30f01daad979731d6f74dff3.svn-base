using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CJToolkit.ProgressBar
{
    [TemplatePart(Name = Root, Type = typeof(Border))]
    [TemplatePart(Name = FillPart, Type = typeof(Border))]
    public class LinearProgressBar : ProgressBarBase, IDisposable
    {
        private const int LoopingTime = 1500;
        private const int NonLoopingTime = 500;

        #region TemplatePart Name

        private const String Root = "Root";
        private const String FillPart = "FillPart";

        #endregion

        #region TemplatePart

        private Border _root;
        private Border _fillPart;

        #endregion

        #region Constructor

        public LinearProgressBar()
        {
            DefaultStyleKey = typeof(LinearProgressBar);
        }

        ~LinearProgressBar()
        {
            Dispose();
        }

        public void Dispose()
        {

        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _root = GetTemplateChild(Root) as Border;
            _fillPart = GetTemplateChild(FillPart) as Border;
            SizeChanged += (sender, e) =>
            {
                if (!UseRoundCorner) return;
                _fillPart.CornerRadius = new CornerRadius(e.NewSize.Height / 2);
                _root.CornerRadius = new CornerRadius(e.NewSize.Height / 2);
                LayoutUpdate();
            };
            LayoutUpdate();
        }

        internal override void IsLoopingChanged()
        {
            base.IsLoopingChanged();
            if (IsLooping)
            {
                GenerateLoopingStoryBoard();
                if (ProgressStoryboard != null)
                    ProgressStoryboard.Begin();
            }
            else
            {
                FillRateChanged(FillRate);
            }
        }

        internal override void FillRateChanged(double newValue, double oldValue = 0)
        {
            base.FillRateChanged(newValue, oldValue);
            if (IsLooping) return;
            GenerateNonLoopingStoryBoard(newValue, oldValue);
            if (ProgressStoryboard != null)
                ProgressStoryboard.Begin();
        }

        #endregion

        #region 私有方法

        void LayoutUpdate()
        {
            IsLoopingChanged();
            ChangeRoundCorner();
        }

        void GenerateLoopingStoryBoard()
        {
            if (_fillPart == null || _root == null || _root.ActualWidth.Equals(double.NaN)) return;
            ProgressStoryboard = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };
            var da = new DoubleAnimation
            {
                From = 0,
                To = _root.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(LoopingTime)
            };
            Storyboard.SetTarget(da, _fillPart);
            Storyboard.SetTargetProperty(da, new PropertyPath("(FrameworkElement.Width)"));
            ProgressStoryboard.Children.Add(da);
        }

        void GenerateNonLoopingStoryBoard(double newValue, double oldValue = 0)
        {
            if (IsLooping || _root == null || _fillPart == null) return;
            if (ProgressStoryboard != null && ProgressStoryboard.GetCurrentState() == ClockState.Active)
                ProgressStoryboard.SkipToFill();
            ProgressStoryboard = new Storyboard();
            var da = new DoubleAnimation
            {
                From = _root.ActualWidth * oldValue,
                To = _root.ActualWidth * newValue,
                Duration = TimeSpan.FromMilliseconds(NonLoopingTime),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(da, _fillPart);
            Storyboard.SetTargetProperty(da, new PropertyPath("(FrameworkElement.Width)"));
            ProgressStoryboard.Children.Add(da);
            ProgressStoryboard.Completed += ProgressStoryboardCompleted;
        }

        void ProgressStoryboardCompleted(object sender, EventArgs e)
        {
            ((Storyboard)sender).Completed -= ProgressStoryboardCompleted;
            _fillPart.Width = _root.ActualWidth * FillRate;
        }

        void ChangeRoundCorner()
        {
            if (_fillPart == null || _root == null) return;
            _fillPart.CornerRadius = UseRoundCorner ? new CornerRadius(DesiredSize.Height / 2) : new CornerRadius(0);
            _root.CornerRadius = UseRoundCorner ? new CornerRadius(DesiredSize.Height / 2) : new CornerRadius(0);

        }

        #endregion

        #region 依赖属性

        public static readonly DependencyProperty UseRoundCornerProperty =
            DependencyProperty.Register("UseRoundCorner", typeof(Boolean), typeof(LinearProgressBar),
                                        new PropertyMetadata(true, OnUseRoundCornerChanged));

        public Boolean UseRoundCorner
        {
            get { return (Boolean)GetValue(UseRoundCornerProperty); }
            set { SetValue(UseRoundCornerProperty, value); }
        }

        private static void OnUseRoundCornerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((LinearProgressBar)obj).ChangeRoundCorner();
        }

        #endregion

    }
}
