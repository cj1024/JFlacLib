using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Expression.Shapes;

namespace CJToolkit.ProgressBar
{
    [TemplatePart(Name=RateText,Type = typeof(TextBlock))]
    [TemplatePart(Name = BackgroundCircle, Type = typeof(Arc))]
    [TemplatePart(Name = ForegroundCircle,Type = typeof(Arc))]
    public class CircleProgressBar : ProgressBarBase, IDisposable
    {
        private const int LoopingTime = 1000;
        private const int NonLoopingTime = 500;

        #region TemplatePart Name

        private const String ForegroundCircle = "ForegroundCircle";
        private const String BackgroundCircle = "BackgroundCircle";
        private const String RateText = "RateText";

        #endregion

        #region TemplatePart

        private Arc _foregroundCircle;
        private Arc _backgroundCircle;
        private TextBlock _rateText;

        #endregion

        #region Constructor

        public CircleProgressBar()
        {
            DefaultStyleKey = typeof (CircleProgressBar);
        }

        ~CircleProgressBar()
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
            _foregroundCircle = GetTemplateChild(ForegroundCircle) as Arc;
            _backgroundCircle = GetTemplateChild(BackgroundCircle) as Arc;
            _rateText = GetTemplateChild(RateText) as TextBlock;
            LayoutUpdate();
        }

        internal override void IsLoopingChanged()
        {
            if (IsLooping)
            {
                if (_rateText != null)
                    _rateText.Visibility = Visibility.Collapsed;
                GenerateLoopingStoryBoard();
                if (ProgressStoryboard == null) return;
                ProgressStoryboard.Begin();
            }
            else
            {
                if (_rateText != null)
                {
                    _rateText.Visibility = Visibility.Visible;
                    _rateText.Text = String.Format("{0:0.0}%", FillRate * 100);
                }
                FillRateChanged(FillRate);
            }
        }

        internal override void FillRateChanged(double newValue, double oldValue = 0)
        {
            if (IsLooping) return;
            GenerateNonLoopingStoryBoard(newValue, oldValue);
            if (ProgressStoryboard != null)
                ProgressStoryboard.Begin();
        }

        #endregion

        #region 依赖属性

        public static readonly DependencyProperty ArcForegroundProperty =
            DependencyProperty.Register("ArcForeground", typeof (Brush), typeof (CircleProgressBar), new PropertyMetadata(default(Brush),OnArcForegroundChanged));

        public Brush ArcForeground
        {
            get { return (Brush) GetValue(ArcForegroundProperty); }
            set { SetValue(ArcForegroundProperty, value); }
        }

        static void OnArcForegroundChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            ((CircleProgressBar)obj).ArcForegroundChanged();
        }

        public static readonly DependencyProperty ArcBackgroundProperty =
            DependencyProperty.Register("ArcBackground", typeof (Brush), typeof (CircleProgressBar), new PropertyMetadata(default(Brush),OnArcBackgroundChanged));

        public Brush ArcBackground
        {
            get { return (Brush) GetValue(ArcBackgroundProperty); }
            set { SetValue(ArcBackgroundProperty, value); }
        }

        static void OnArcBackgroundChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            ((CircleProgressBar)obj).ArcBackgroundChanged();
        }

        public static readonly DependencyProperty ArcThicknessProperty =
            DependencyProperty.Register("ArcThickness", typeof (double), typeof (CircleProgressBar), new PropertyMetadata(default(double),OnArcThicknessChanged));

        public double ArcThickness
        {
            get { return (double) GetValue(ArcThicknessProperty); }
            set { SetValue(ArcThicknessProperty, value); }
        }

        private static void OnArcThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (((double) e.NewValue) < 0)
            {
                ((CircleProgressBar)obj).ArcThickness = (double)e.OldValue;
                throw (new ArgumentOutOfRangeException("e", "ArcThickness必须大于0"));
            }
            ((CircleProgressBar)obj).ArcThicknessChanged();

        }

        #endregion

        #region 私有方法
        
        void LayoutUpdate()
        {
            ArcForegroundChanged();
            ArcBackgroundChanged();
            ArcThicknessChanged();
            IsLoopingChanged();
        }

        void ArcForegroundChanged()
        {
            if(_foregroundCircle==null) return;
            _foregroundCircle.Fill = ArcForeground;
        }

        void ArcBackgroundChanged()
        {
            if (_backgroundCircle == null) return;
            _backgroundCircle.Fill = ArcBackground;
        }

        void ArcThicknessChanged()
        {
            if (_backgroundCircle == null||_foregroundCircle==null) return;
            _foregroundCircle.ArcThickness = ArcThickness;
            _backgroundCircle.ArcThickness = ArcThickness;
        }

        void GenerateLoopingStoryBoard()
        {
            if(_foregroundCircle==null) return;
            if(ProgressStoryboard!=null&&ProgressStoryboard.GetCurrentState()==ClockState.Active)
                ProgressStoryboard.Stop();
            ProgressStoryboard=new Storyboard {RepeatBehavior = RepeatBehavior.Forever};
            var dakfs = new DoubleAnimationUsingKeyFrames();
            var dakf = new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 };
            dakfs.KeyFrames.Add(dakf);
            dakf = new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(LoopingTime), Value = 360 };
            dakfs.KeyFrames.Add(dakf);
            Storyboard.SetTarget(dakfs,_foregroundCircle);
            Storyboard.SetTargetProperty(dakfs,new PropertyPath("(Acr.EndAngle)"));
            ProgressStoryboard.Children.Add(dakfs);
            dakfs = new DoubleAnimationUsingKeyFrames();
            dakf = new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(LoopingTime), Value = 0 };
            dakfs.KeyFrames.Add(dakf);
            dakf = new EasingDoubleKeyFrame
                {KeyTime = TimeSpan.FromMilliseconds(LoopingTime + LoopingTime), Value = 360};
            dakfs.KeyFrames.Add(dakf);
            Storyboard.SetTarget(dakfs, _foregroundCircle);
            Storyboard.SetTargetProperty(dakfs, new PropertyPath("(Acr.StartAngle)"));
            ProgressStoryboard.Children.Add(dakfs);
        }

        void GenerateNonLoopingStoryBoard(double newValue, double oldValue = 0)
        {
            const int step = 10;
            if (_foregroundCircle == null) return;
            if (IsLooping) return;
            if (ProgressStoryboard != null && ProgressStoryboard.GetCurrentState() == ClockState.Active)
                ProgressStoryboard.SkipToFill();
            ProgressStoryboard = new Storyboard();
            var da = new DoubleAnimation
                {
                    From = oldValue * 360,
                    To = newValue * 360,
                    Duration = TimeSpan.FromMilliseconds(NonLoopingTime),
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
                };
            Storyboard.SetTarget(da,_foregroundCircle);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Acr.EndAngle)"));
            ProgressStoryboard.Children.Add(da);
            if (_rateText != null)
            {
                var dakfs = new ObjectAnimationUsingKeyFrames();
                for (int i = 0; i <= step; i++)
                {
                    var current = String.Format("{0:0.#}%", (oldValue + (newValue - oldValue)/step*i)*100);
                    var dakf = new DiscreteObjectKeyFrame
                        {KeyTime = TimeSpan.FromMilliseconds(NonLoopingTime/step*i), Value = current};
                    dakfs.KeyFrames.Add(dakf);
                }
                Storyboard.SetTarget(dakfs, _rateText);
                Storyboard.SetTargetProperty(dakfs, new PropertyPath("(TextBlock.Text)"));
                ProgressStoryboard.Children.Add(dakfs);
            }
            ProgressStoryboard.Completed += ProgressStoryboardCompleted;
        }

        void ProgressStoryboardCompleted(object sender, EventArgs e)
        {
            ((Storyboard)sender).Completed -= ProgressStoryboardCompleted;
            _foregroundCircle.EndAngle = 360*FillRate < 360 ? 360*FillRate : 360;
            if (_rateText != null)
                _rateText.Text = String.Format("{0:0.#}%", FillRate*100);
        }

        #endregion


    }
}
