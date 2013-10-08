using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;

namespace CJToolkit.PivotExtention
{
    public class AnimationPivot:Pivot
    {
        #region 参数常量

        private const int FlipDuration = 500;
        private const int FadeDuration = 500;

        #endregion

        private AnimationDirection? _animationDirection;

        #region Constructor

        public AnimationPivot()
        {
            _animationDirection=AnimationDirection.Center;
            //获取Pivot是向左滑动还是向右滑动
            ManipulationCompleted += (sender, e) =>
                {
                    _animationDirection = e.TotalManipulation.Translation.X > 0
                                              ? AnimationDirection.Left
                                              : AnimationDirection.Right;
                };
        }

        #endregion
        
        #region Override

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            foreach (var newItem in e.NewItems)
            {
                var uiElement = newItem as UIElement;
                if (uiElement != null)
                    uiElement.ManipulationDelta += (sender, eve) =>
                        {
                            eve.Handled = true;
                        };
            }
        }
        
        protected override void UpdateItemVisibility(UIElement element, bool toVisible)
        {
            base.UpdateItemVisibility(element, toVisible);
            element.Opacity = toVisible ? 1 : 0;
            PlayAnimationOnPage(element, toVisible);
        }

        protected virtual void PlayAnimationOnPage(UIElement element,bool toVisible)
        {
            var storyBoard = new Storyboard();
            element.Projection = new PlaneProjection { CenterOfRotationZ = 0 - element.RenderSize.Width / 2 };
            if(toVisible)//将要被展示的页面
            {
                //
                Canvas.SetZIndex(element,1);
                //翻转的动画
                var da = new DoubleAnimation
                    {
                        From = _animationDirection == AnimationDirection.Right ? -90 : 90,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(FlipDuration)
                    };
                Storyboard.SetTarget(da,element.Projection);
                Storyboard.SetTargetProperty(da, new PropertyPath("(PlaneProjection.RotationY)"));
                storyBoard.Children.Add(da);
            }
            else
            {
                if(Canvas.GetZIndex(element)!=1) return;
                Canvas.SetZIndex(element, 0);
                //翻转的动画
                var da = new DoubleAnimation
                {
                    To = _animationDirection == AnimationDirection.Right ? 90 :-90,
                    From = 0,
                    Duration = TimeSpan.FromMilliseconds(FlipDuration)
                };
                Storyboard.SetTarget(da, element.Projection);
                Storyboard.SetTargetProperty(da, new PropertyPath("(PlaneProjection.RotationY)"));
                storyBoard.Children.Add(da);
                //透明度
                da = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(FadeDuration)
                    };
                Storyboard.SetTarget(da, element);
                Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
                storyBoard.Children.Add(da);
            }
            IsHitTestVisible = false;
            storyBoard.Begin();
            storyBoard.Completed += (sender, e) => { IsHitTestVisible = true; };
        }

        #endregion


    }
}
