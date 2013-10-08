using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Expression.Shapes;

namespace CJToolkit.RateControl
{
    [TemplatePart(Name = CrossPart,Type = typeof(RegularPolygon))]
    public class Cross : Control
    {
        #region TemplatePart Name

        private const String CrossPart = "CrossPart";

        #endregion

        #region TemplatePart

        private RegularPolygon _crossPart;

        #endregion

        #region 一些常量

        private const double DefaultStrokeness = 3;

        #endregion

        #region Constructor

        public Cross()
        {
            DefaultStyleKey = typeof (Cross);
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _crossPart = GetTemplateChild(CrossPart) as RegularPolygon;
            ChangeStrokeThickness();
        }

        #endregion

        #region 依赖属性

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Cross),
                                        new PropertyMetadata(DefaultStrokeness, OnStrokeThicknessChanged));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        private static void OnStrokeThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((Cross)obj).ChangeStrokeThickness();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 改变边框粗细
        /// </summary>
        private void ChangeStrokeThickness()
        {
            if (_crossPart == null) return;
            _crossPart.StrokeThickness = double.IsNaN(StrokeThickness) || StrokeThickness < 0 ? 0 : StrokeThickness;
        }

        #endregion

    }
}
