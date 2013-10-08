using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CJToolkit.RateControl
{
    public class RateControlBase:Control
    {

        #region 依赖属性

        public static readonly DependencyProperty RateProperty =
            DependencyProperty.Register("Rate", typeof(int), typeof(RateControlBase), new PropertyMetadata(0, OnRatePropertyChanged));

        public int Rate
        {
            get { return (int) GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }

        static void OnRatePropertyChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < ((RateControlBase)obj).Min || (int)e.NewValue > ((RateControlBase)obj).Max)
                throw new ArgumentOutOfRangeException("e", String.Format("Rate should be in range of {0} - {1}", ((RateControlBase)obj).Min, ((RateControlBase)obj).Max));
            ((RateControlBase)obj).RateChanged(e);
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof (int), typeof (RateControlBase), new PropertyMetadata(5,OnMaxChanged));

        protected int Max
        {
            get { return (int) GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        static void OnMaxChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((RateControlBase)obj).MaxChanged();
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof (int), typeof (RateControlBase), new PropertyMetadata(0,OnMinChanged));

        protected int Min
        {
            get { return (int) GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        static void OnMinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((RateControlBase)obj).MinChanged();
        }

        public static readonly DependencyProperty RateChangedCommandProperty =
            DependencyProperty.Register("RateChangedCommand", typeof(ICommand), typeof(StarRateControl),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof (bool), typeof (RateControlBase), new PropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public ICommand RateChangedCommand
        {
            get { return (ICommand)GetValue(RateChangedCommandProperty); }
            set { SetValue(RateChangedCommandProperty, value); }
        }

        #endregion

        #region 抽象方法

        protected virtual void RateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (OnRateChanged != null)
                OnRateChanged.Invoke(this, e);
            if (RateChangedCommand != null)
                RateChangedCommand.Execute(this);
        }

        protected virtual void MaxChanged() { }

        protected virtual void MinChanged() { }

        #endregion

        #region 事件

        public delegate void RateChangedHandler(object sender, DependencyPropertyChangedEventArgs e);

        public event RateChangedHandler OnRateChanged;

        #endregion

    }
}
