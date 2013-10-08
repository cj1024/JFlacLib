﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Expression.Shapes;

namespace CJToolkit.RateControl
{
    public class StarRateControl:RateControlBase
    {
        #region 私有变量

        private const int MinValue = 0;
        private const int MaxValue = 5;
        private readonly Brush _transparnBrush = new SolidColorBrush(Colors.Transparent);
        private IList<RegularPolygon> _itemList;

        #endregion
        
        #region Constructor

        public StarRateControl()
        {
            DefaultStyleKey = typeof (StarRateControl);
            Min = MinValue;
            Max = MaxValue;
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _itemList = ViewTree.ViewTreeHelper.GetVisualChildCollection<RegularPolygon>(this);
            foreach (var regularPolygon in _itemList)
            {
                ((Grid)regularPolygon.Parent).MouseEnter += RegularPolygonMouseEnter;
            }
            UpdateRateLayout();
        }

        protected override void RateChanged(DependencyPropertyChangedEventArgs e)
        {
            base.RateChanged(e);
            UpdateRateLayout();
        }

        #endregion

        #region 私有方法

        private void RegularPolygonMouseEnter(object sender, MouseEventArgs e)
        {
            if(!IsReadOnly)
                Rate = _itemList.IndexOf((RegularPolygon) ((Grid) sender).Children[0]) + 1;
        }

        protected virtual void UpdateRateLayout()
        {
            if (_itemList == null) return;
            for (var i = Min; i < Max; i++)
            {
                if (Rate > i && _itemList[i].Fill == _transparnBrush)
                    AnimeBrushOpacity(_itemList[i]);
                _itemList[i].Fill = Rate > i ? Foreground : _transparnBrush;
            }
        }

        private void AnimeBrushOpacity(UIElement obj)
        {
            var transform = new ScaleTransform{CenterX = 0.5,CenterY = 0.5};
            obj.RenderTransform = transform;
            var anime = new Storyboard();
            var da = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };
            Storyboard.SetTarget(da, obj);
            Storyboard.SetTargetProperty(da, new PropertyPath("UIElement.Opacity"));
            anime.Children.Add(da);
            da = new DoubleAnimation
                {
                    From = 0.5,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new BackEase {EasingMode = EasingMode.EaseOut}
                };
            Storyboard.SetTarget(da, transform);
            Storyboard.SetTargetProperty(da, new PropertyPath("ScaleX"));
            anime.Children.Add(da);
            da = new DoubleAnimation
            {
                From = 0.5,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(da, transform);
            Storyboard.SetTargetProperty(da, new PropertyPath("ScaleY"));
            anime.Children.Add(da);
            anime.Begin();
        }

        #endregion

    }
}
