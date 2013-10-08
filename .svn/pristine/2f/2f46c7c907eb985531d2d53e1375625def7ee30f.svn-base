using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CJToolkit.RateControl
{
    [TemplatePart(Name = HeartPart, Type = typeof (Path))]
    [TemplatePart(Name = PathFigureCollection, Type = typeof (PathFigureCollection))]
    public class Heart : Control
    {
        #region TemplatePartName

        private const String HeartPart = "HeartPart";
        private const String PathFigureCollection = "PathFigureCollection";

        #endregion

        #region TemplatePart

        private Path _heartPart;
        private PathFigureCollection _pathFigureCollection;

        #endregion
        
        #region 一些常量

        private readonly List<Point> _rightPathPoints = new List<Point>
            {
                new Point(0.5, 0.25),
                new Point(0.125, 0),
                new Point(0.1, 0.4),
                new Point(0.1, 0.5),
                new Point(0.125, 0.5),
                new Point(0.5, 0.875)
            };

        private readonly List<Point> _leftPathPoints = new List<Point>
            {
                new Point(0.5, 0.25),
                new Point(0.875, 0),
                new Point(0.9, 0.4),
                new Point(0.9, 0.5),
                new Point(0.875, 0.5),
                new Point(0.5, 0.875)
            };

        private readonly Point _startPoint = new Point(0.5, 0.25);

        private const double DefaultStrokeness = 3;

        #endregion
        
        #region Constructor

        public Heart()
        {
            DefaultStyleKey = typeof (Heart);
            SizeChanged += HeartSizeChanged;
        }

        #endregion

        #region 依赖属性

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (Heart),
                                        new PropertyMetadata(DefaultStrokeness, OnStrokeThicknessChanged));

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        private static void OnStrokeThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((Heart)obj).ChangeStrokeThickness();
        }

        #endregion
        
        #region 事件处理

        /// <summary>
        /// Size变化后需要调整Heart尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangePathPoint();
        }

        #endregion
        
        #region 私有函数

        /// <summary>
        /// 按高宽改变PathPoint
        /// </summary>
        private void ChangePathPoint()
        {
            if (double.IsNaN(ActualHeight) || double.IsNaN(ActualWidth)) return;
            if (_pathFigureCollection == null) return;
            _pathFigureCollection.Clear();
            //左半边
            var figure = new PathFigure
                {
                    StartPoint = new Point(ActualWidth*_startPoint.X, ActualHeight*_startPoint.Y)
                };
            var bezier = new PolyBezierSegment();
            foreach (var leftPathPoint in _leftPathPoints)
            {
                bezier.Points.Add(new Point(ActualWidth*leftPathPoint.X, ActualHeight*leftPathPoint.Y));
            }
            figure.Segments.Add(bezier);
            _pathFigureCollection.Add(figure);
            //右半边
            figure = new PathFigure
                {
                    StartPoint = new Point(ActualWidth*_startPoint.X, ActualHeight*_startPoint.Y)
                };
            bezier = new PolyBezierSegment();
            foreach (var rightPathPoint in _rightPathPoints)
            {
                bezier.Points.Add(new Point(ActualWidth*rightPathPoint.X, ActualHeight*rightPathPoint.Y));
            }
            figure.Segments.Add(bezier);
            _pathFigureCollection.Add(figure);
        }

        /// <summary>
        /// 改变边框粗细
        /// </summary>
        private void ChangeStrokeThickness()
        {
            if(_heartPart==null) return;
            _heartPart.StrokeThickness = double.IsNaN(StrokeThickness) || StrokeThickness < 0 ? 1 : StrokeThickness;
        }

        #endregion
        
        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _heartPart = GetTemplateChild(HeartPart) as Path;
            _pathFigureCollection = GetTemplateChild(PathFigureCollection) as PathFigureCollection;
            ChangePathPoint();
            ChangeStrokeThickness();
        }

        #endregion

    }
}
