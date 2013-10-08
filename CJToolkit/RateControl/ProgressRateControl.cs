using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CJToolkit.ProgressBar;

namespace CJToolkit.RateControl
{
    [TemplatePart(Name = Root,Type = typeof(Grid))]
    [TemplatePart(Name = Progress,Type = typeof(LinearProgressBar))]
    public class ProgressRateControl:RateControlBase
    {
        #region TemplatePart Name

        private const String Root = "Root";
        private const String Progress = "Progress";

        #endregion

        #region TemplatePart

        private Grid _root;
        private LinearProgressBar _progress;

        #endregion

        #region Constructor

        public ProgressRateControl()
        {
            DefaultStyleKey = typeof (ProgressRateControl);
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _root = GetTemplateChild(Root) as Grid;
            _progress = GetTemplateChild(Progress) as LinearProgressBar;
            if (_root != null && _progress != null)
            {
                _root.MouseMove += (sender, e) =>
                    {
                        var tempRate = e.GetPosition(_root).X/_root.ActualWidth;
                        Rate = (int) Math.Round(tempRate*(Max - Min));
                    };
                _root.Tap += (sender, e) =>
                    {
                        var tempRate = e.GetPosition(_root).X/_root.ActualWidth;
                        Rate = (int) Math.Round(tempRate*(Max - Min));
                    };
            }
        }

        protected override void RateChanged(DependencyPropertyChangedEventArgs e)
        {
            base.RateChanged(e);
            if(_progress!=null)
                _progress.FillRate = (1d * Rate) / (Max - Min);
        }

        #endregion


    }
}
