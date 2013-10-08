using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;

namespace CJToolkit.ProgressBar
{
    public class ProgressBarBase : PerformanceProgressBar
    {
        
        internal Storyboard ProgressStoryboard;

        public ProgressBarBase()
        {
            CacheMode=new BitmapCache();
        }

        #region 依赖属性
        
        public static readonly DependencyProperty IsLoopingProperty =
            DependencyProperty.Register("IsLooping", typeof(Boolean), typeof(ProgressBarBase), new PropertyMetadata(default(Boolean), OnIsLoopingChanged));

        public Boolean IsLooping
        {
            get { return (Boolean)GetValue(IsLoopingProperty); }
            set { SetValue(IsLoopingProperty, value); }
        }

        static void OnIsLoopingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBarBase)obj).IsLoopingChanged();
        }

        public static readonly DependencyProperty FillRateProperty =
            DependencyProperty.Register("FillRate", typeof(double), typeof(ProgressBarBase), new PropertyMetadata(default(double), OnFillRateChanged));

        public double FillRate
        {
            get { return (double)GetValue(FillRateProperty); }
            set { SetValue(FillRateProperty, value); }
        }

        static void OnFillRateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue < 0 || (double)e.NewValue > 1)
            {
                ((ProgressBarBase)obj).FillRate = (double)e.OldValue;
                throw (new ArgumentOutOfRangeException("e", "FillRate必须大于0且小于1"));
            }
            ((ProgressBarBase) obj).FillRateChanged((double) e.NewValue, (double) e.OldValue);
        }

        #endregion
        
        #region 私有方法


        internal virtual void IsLoopingChanged()
        {
            
        }


        internal virtual void FillRateChanged(double newValue, double oldValue = 0)
        {

        }
        
        #endregion

    }
}
