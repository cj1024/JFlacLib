using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CJToolkit.InputControl
{
    [TemplatePart(Name = Watermark, Type = typeof (TextBlock))]
    public class WatermarkTextbox : TextBox
    {

        #region TemplatePart Name

        private const String Watermark = "Watermark";

        #endregion

        #region TemplatePart

        private TextBlock _watermark;

        #endregion

        #region 私有变量

        private bool _getFocus;

        #endregion

        #region Constructor

        public WatermarkTextbox()
        {
            DefaultStyleKey = typeof(WatermarkTextbox);
            _getFocus = false;
            TextChanged += (sender, e) =>
                {
                    RealText = Text;
                    UpdateWaterMarkVisibility();
                    if (UpdateBindingOnChange)
                    {
                        var bindingExpression = GetBindingExpression(TextProperty);
                        if (bindingExpression != null)
                            bindingExpression.UpdateSource();
                    }
                    if (MaxLength <= 0 || Text.Length <= MaxLength) return;
                    Text = Text.Substring(0, MaxLength);
                    Select(MaxLength, 0);
                };
        }

        #endregion

        #region Override
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _watermark = GetTemplateChild(Watermark) as TextBlock;
            UpdateUI();
            UpdateWaterMarkVisibility();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _getFocus = true;
            UpdateWaterMarkVisibility();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _getFocus = false;
            UpdateWaterMarkVisibility();
        }
        
        #endregion

        #region 依赖属性

        private static readonly DependencyProperty RealTextProperty =
            DependencyProperty.Register("RealText", typeof(string), typeof(WatermarkTextbox),
                                        new PropertyMetadata(OnRealTextChanged));

        private string RealText
        {
            set { SetValue(RealTextProperty, value); }
        }

        private static void OnRealTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var sender = (WatermarkTextbox)obj;
            if (sender.RealTextChanged != null)
            {
                sender.RealTextChanged(sender, e);
            }
            if (sender.TextChangedCommand != null)
            {
                sender.TextChangedCommand.Execute(e);
            }
        }

        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("WaterMark", typeof(string), typeof(WatermarkTextbox),
                                        new PropertyMetadata("您还没有输入", UIPropertyChanged));

        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        public static readonly DependencyProperty WaterMarkGroundProperty =
            DependencyProperty.Register("WaterMarkGround", typeof(Brush), typeof(WatermarkTextbox),
                                        new PropertyMetadata(new SolidColorBrush(Colors.Gray), UIPropertyChanged));

        public Brush WaterMarkGround
        {
            get { return (Brush)GetValue(WaterMarkGroundProperty); }
            set { SetValue(WaterMarkGroundProperty, value); }
        }

        public static readonly DependencyProperty HideOnFocusProperty =
            DependencyProperty.Register("HideOnFocus", typeof(bool), typeof(WatermarkTextbox),
                                        new PropertyMetadata(true, UIPropertyChanged));

        public bool HideOnFocus
        {
            get { return (bool)GetValue(HideOnFocusProperty); }
            set { SetValue(HideOnFocusProperty, value); }
        }

        private static void UIPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkTextbox)obj).UpdateUI();
        }

        public static readonly DependencyProperty UpdateBindingOnChangeProperty =
            DependencyProperty.Register("UpdateBindingOnChange", typeof (bool), typeof (WatermarkTextbox), new PropertyMetadata(true));

        public bool UpdateBindingOnChange
        {
            get { return (bool) GetValue(UpdateBindingOnChangeProperty); }
            set { SetValue(UpdateBindingOnChangeProperty, value); }
        }

        #endregion

        #region 私有方法

        private void UpdateUI()
        {
            if (_watermark != null)
            {
                _watermark.Text = WaterMark;
                _watermark.Foreground = WaterMarkGround;
            }
            UpdateWaterMarkVisibility();
        }

        private void UpdateWaterMarkVisibility()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                if (_watermark != null)
                {
                    _watermark.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (_watermark != null)
                {
                    if (HideOnFocus && _getFocus)
                        _watermark.Visibility = Visibility.Collapsed;
                    else
                        _watermark.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion

        #region 自定义事件和Command

        #region RealText变化

        public delegate void RealTextChangedHandler(object sender, DependencyPropertyChangedEventArgs e);

        /// <summary>
        /// 输入文本发生变化的事件
        /// </summary>
        public event RealTextChangedHandler RealTextChanged;

        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.Register("TextChangedCommand", typeof(ICommand), typeof(WatermarkTextbox),
                                        new PropertyMetadata(default(ICommand)));

        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }

        #endregion

        #endregion

    }
}
