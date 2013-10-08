using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Practices.Prism.Commands;

namespace WP7FlacPlayer.Controls
{
    /// <summary>
    /// 有添动画效果的ListBox
    /// </summary>
    public class AnimedListBox:ListBox
    {
        /// <summary>
        /// 每个Item动画播放完之后要执行的Command
        /// </summary>
        private Dictionary<Storyboard, AnimedListBoxCommandPair> _storyBoardCompletedCommands;

        /// <summary>
        /// 已生成过动画的Item
        /// </summary>
        private List<WeakReference> _playedItem;
        
        /// <summary>
        /// IsOnCurrentPage要用
        /// </summary>
        private FrameworkElement _visual;

        public static readonly DependencyProperty PerDelayProperty =
            DependencyProperty.Register("PerDelay", typeof (int), typeof (AnimedListBox), new PropertyMetadata(100));

        public int PerDelay
        {
            get { return (int) GetValue(PerDelayProperty); }
            set { SetValue(PerDelayProperty, value); }
        }

        private void Initial()
        {
            _storyBoardCompletedCommands = new Dictionary<Storyboard, AnimedListBoxCommandPair>();
            _playedItem = new List<WeakReference>();
        }

        public AnimedListBox()
        {
            Initial();
        }
        
        /// <summary>
        /// 判断Item是否在屏幕上显示
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool IsOnCurrentPage(ListBoxItem item)
        {
            if (_visual == null)
            {
                var ich = new ItemsControlHelper(this);
                ScrollContentPresenter scp = ich.ScrollHost == null ? null : ich.ScrollHost.GetVisualDescendants().OfType<ScrollContentPresenter>().FirstOrDefault();
                _visual = (ich.ScrollHost == null) ? null : ((scp == null) ? ich.ScrollHost : ((FrameworkElement)scp));
            }

            if (_visual == null)
                return true;

            var itemsHostRect = new Rect(0.0, 0.0, _visual.ActualWidth, _visual.ActualHeight);
            if (item == null)
            {
                return false;
            }

            var transform = item.TransformToVisual(_visual);

            var listBoxItemRect = new Rect(transform.Transform(new Point()), transform.Transform(new Point(item.ActualWidth, item.ActualHeight)));

            //新增的为了弥补ScrollToTop后动画失效的修正，但不保证效果
            if (double.IsNaN(item.ActualHeight) || item.ActualHeight <= 0)
                return true;

            return ((listBoxItemRect.Bottom + 100 >= itemsHostRect.Top) && (listBoxItemRect.Top - 100 <= itemsHostRect.Bottom));
            //return ((itemsHostRect.Top <= listBoxItemRect.Bottom) && (listBoxItemRect.Top <= itemsHostRect.Bottom));
        }

        /// <summary>
        /// 在ListBox为每一项准备容器时添加动画并播放
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if(Visibility==Visibility.Collapsed) return;
            if (element is ListBoxItem && _playedItem.All(s => s.Target != item) && IsOnCurrentPage(element as ListBoxItem))
            {
                
                var sb = new Storyboard();
                var listBoxItem = element as ListBoxItem;
                _playedItem.Add(new WeakReference(item));
                listBoxItem.Opacity = 0;
                //透明动画部分
                var da = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(600),
                    BeginTime = TimeSpan.FromMilliseconds(100),
                    To = 1,
                    From = 0
                };
                Storyboard.SetTarget(da, listBoxItem);
                Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
                sb.Children.Add(da);
                var translateTransform = new TranslateTransform();
                listBoxItem.RenderTransform = translateTransform;
                da = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromMilliseconds(600),
                        BeginTime = TimeSpan.FromMilliseconds(100),
                        To = 0,
                        From = 0 - (double.IsNaN(listBoxItem.Width)?480:listBoxItem.ActualWidth),
                        EasingFunction = new PowerEase{EasingMode = EasingMode.EaseOut}
                    };
                Storyboard.SetTarget(da,translateTransform);
                Storyboard.SetTargetProperty(da,new PropertyPath("TranslateTransform.X"));
                sb.Children.Add(da);
                /* 旧的动画
                var pp = new PlaneProjection { CenterOfRotationX = 0, CenterOfRotationY = 0 };
                listBoxItem.Projection = pp;
                //x轴翻转动画部分

                da = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(500),
                    BeginTime =
                        TimeSpan.FromMilliseconds(0),
                    To = 0,
                    From = -90,
                };
                Storyboard.SetTarget(da, listBoxItem);
                Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
                sb.Children.Add(da);
                //y轴翻转动画部分
                da = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(500),
                    BeginTime =
                        TimeSpan.FromMilliseconds(0),
                    To = 0,
                    From = 90,
                };
                Storyboard.SetTarget(da, listBoxItem);
                Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationX)"));
                sb.Children.Add(da);*/

                int delay = Math.Min(_storyBoardCompletedCommands.Count, Items.IndexOf(item));
                sb.BeginTime = TimeSpan.FromMilliseconds((delay + 1)*PerDelay);
                sb.Begin();
                _storyBoardCompletedCommands.Add(sb, new AnimedListBoxCommandPair
                    {
                        Command = new DelegateCommand(() =>
                            {
                                listBoxItem.Opacity = 1;
                            }),
                        Item = item
                    });
                sb.Completed += SbCompleted;
            }

            //清理不存在的Item残留，释放内存
            if (_storyBoardCompletedCommands.Count > 6)
            {
                var temp = new List<WeakReference>(_playedItem);
                foreach (var obj in temp)
                {
                    if (!Items.Contains(obj.Target))
                        _playedItem.Remove(obj);
                }
                var dtemp = new Dictionary<Storyboard, AnimedListBoxCommandPair>(_storyBoardCompletedCommands);
                foreach (var sb in dtemp.Keys)
                {
                    if (!Items.Contains(dtemp[sb].Item))
                    {
                        _storyBoardCompletedCommands.Remove(sb);
                        sb.SkipToFill();
                    }
                }
            }
        }

        public void ReSetPlayedItems()
        {
            _playedItem.Clear();
        }

        /// <summary>
        /// 在动画播放完成后还原Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SbCompleted(object sender, EventArgs e)
        {
            var sb = sender as Storyboard;
            if (sb != null&&_storyBoardCompletedCommands.Keys.Contains(sb))
            {
                _storyBoardCompletedCommands[sb].Command.Execute(null);
                _storyBoardCompletedCommands.Remove(sb);
            }
        }

        struct AnimedListBoxCommandPair
        {
            public object Item { get; set; }
            public ICommand Command { get; set; }
        }
    }

    /// <summary>
    /// The ItemContainerGenerator provides useful utilities for ItemsControls.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class ItemsControlHelper
    {
        /// <summary>
        /// Gets or sets the ItemsControl being tracked by the
        /// ItemContainerGenerator.
        /// </summary>
        private ItemsControl ItemsControl { get; set; }

        /// <summary>
        /// A Panel that is used as the ItemsHost of the ItemsControl.  This
        /// property will only be valid when the ItemsControl is live in the
        /// tree and has generated containers for some of its items.
        /// </summary>
        private Panel _itemsHost;

        /// <summary>
        /// Gets a Panel that is used as the ItemsHost of the ItemsControl.
        /// This property will only be valid when the ItemsControl is live in
        /// the tree and has generated containers for some of its items.
        /// </summary>
        public Panel ItemsHost
        {
            get
            {
                // Lookup the ItemsHost if we haven't already cached it.
                if (_itemsHost == null && ItemsControl != null && ItemsControl.ItemContainerGenerator != null)
                {
                    // Get any live container
                    DependencyObject container = ItemsControl.ItemContainerGenerator.ContainerFromIndex(0);
                    if (container != null)
                    {
                        // Get the parent of the container
                        _itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                    }
                    else
                    {
                        _itemsHost = ItemsControl.GetVisualDescendants().OfType<VirtualizingStackPanel>().FirstOrDefault();
                    }
                }

                return _itemsHost;
            }
        }

        /// <summary>
        /// A ScrollViewer that is used to scroll the items in the ItemsHost.
        /// </summary>
        private ScrollViewer _scrollHost;

        /// <summary>
        /// Gets a ScrollViewer that is used to scroll the items in the
        /// ItemsHost.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is linked into multiple projects.")]
        public ScrollViewer ScrollHost
        {
            get
            {
                if (_scrollHost == null)
                {
                    Panel itemsHost = ItemsHost;
                    if (itemsHost != null)
                    {
                        for (DependencyObject obj = itemsHost; obj != ItemsControl && obj != null; obj = VisualTreeHelper.GetParent(obj))
                        {
                            var viewer = obj as ScrollViewer;
                            if (viewer != null)
                            {
                                _scrollHost = viewer;
                                break;
                            }
                        }
                    }
                }
                return _scrollHost;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ItemContainerGenerator.
        /// </summary>
        /// <param name="control">
        /// The ItemsControl being tracked by the ItemContainerGenerator.
        /// </param>
        public ItemsControlHelper(ItemsControl control)
        {
            Debug.Assert(control != null, "control cannot be null!");
            ItemsControl = control;
        }

        /// <summary>
        /// Apply a control template to the ItemsControl.
        /// </summary>
        public void OnApplyTemplate()
        {
            // Clear the cached ItemsHost, ScrollHost
            _itemsHost = null;
            _scrollHost = null;
        }

        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="parentItemContainerStyle">
        /// The ItemContainerStyle for the parent ItemsControl.
        /// </param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is linked into multiple projects.")]
        public static void PrepareContainerForItemOverride(DependencyObject element, Style parentItemContainerStyle)
        {
            // Apply the ItemContainerStyle to the item
            var control = element as Control;
            if (parentItemContainerStyle != null && control != null && control.Style == null)
            {
                control.SetValue(FrameworkElement.StyleProperty, parentItemContainerStyle);
            }

            // Note: WPF also does preparation for ContentPresenter,
            // ContentControl, HeaderedContentControl, and ItemsControl.  Since
            // we don't have any other ItemsControls using this
            // ItemContainerGenerator, we've removed that code for now.  It
            // should be added back later when necessary.
        }

        /// <summary>
        /// Update the style of any generated items when the ItemContainerStyle
        /// has been changed.
        /// </summary>
        /// <param name="itemContainerStyle">The ItemContainerStyle.</param>
        /// <remarks>
        /// Silverlight does not support setting a Style multiple times, so we
        /// only attempt to set styles on elements whose style hasn't already
        /// been set.
        /// </remarks>
        public void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle == null)
            {
                return;
            }

            Panel itemsHost = ItemsHost;
            if (itemsHost == null || itemsHost.Children == null)
            {
                return;
            }

            foreach (UIElement element in itemsHost.Children)
            {
                var obj = element as FrameworkElement;
                if (obj != null && obj.Style == null)
                {
                    obj.Style = itemContainerStyle;
                }
            }
        }

        /// <summary>
        /// Scroll the desired element into the ScrollHost's viewport.
        /// </summary>
        /// <param name="element">Element to scroll into view.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "File is linked across multiple projects and this method is used in some but not others.")]
        public void ScrollIntoView(FrameworkElement element)
        {
            // Get the ScrollHost
            ScrollViewer scrollHost = ScrollHost;
            if (scrollHost == null)
            {
                return;
            }

            // Get the position of the element relative to the ScrollHost
            GeneralTransform transform;
            try
            {
                transform = element.TransformToVisual(scrollHost);
            }
            catch (ArgumentException)
            {
                // Ignore failures when not in the visual tree
                return;
            }
            var itemRect = new Rect(
                transform.Transform(new Point()),
                transform.Transform(new Point(element.ActualWidth, element.ActualHeight)));

            // Scroll vertically
            double verticalOffset = scrollHost.VerticalOffset;
            double verticalDelta = 0;
            double hostBottom = scrollHost.ViewportHeight;
            double itemBottom = itemRect.Bottom;
            if (hostBottom < itemBottom)
            {
                verticalDelta = itemBottom - hostBottom;
                verticalOffset += verticalDelta;
            }
            double itemTop = itemRect.Top;
            if (itemTop - verticalDelta < 0)
            {
                verticalOffset -= verticalDelta - itemTop;
            }
            scrollHost.ScrollToVerticalOffset(verticalOffset);

            // Scroll horizontally
            double horizontalOffset = scrollHost.HorizontalOffset;
            double horizontalDelta = 0;
            double hostRight = scrollHost.ViewportWidth;
            double itemRight = itemRect.Right;
            if (hostRight < itemRight)
            {
                horizontalDelta = itemRight - hostRight;
                horizontalOffset += horizontalDelta;
            }
            double itemLeft = itemRect.Left;
            if (itemLeft - horizontalDelta < 0)
            {
                horizontalOffset -= horizontalDelta - itemLeft;
            }
            scrollHost.ScrollToHorizontalOffset(horizontalOffset);
        }
    }

    public static class VisualTreeExtensions
    {
        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualDescendantsAndSelfIterator(element).Skip(1);
        }

        private static IEnumerable<DependencyObject> GetVisualDescendantsAndSelfIterator(DependencyObject element)
        {
            Debug.Assert(element != null, "element should not be null!");

            var remaining = new Queue<DependencyObject>();
            remaining.Enqueue(element);

            while (remaining.Count > 0)
            {
                DependencyObject obj = remaining.Dequeue();
                yield return obj;

                foreach (DependencyObject child in obj.GetVisualChildren())
                {
                    remaining.Enqueue(child);
                }
            }
        }

        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject target)
           where T : DependencyObject
        {
            return GetVisualChildren(target).Where(child => child is T).Cast<T>();
        }

        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject target, bool strict)
            where T : DependencyObject
        {
            return GetVisualChildren(target, strict).Where(child => child is T).Cast<T>();
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject target, bool strict)
        {
            int count = VisualTreeHelper.GetChildrenCount(target);
            if (count == 0)
            {
                if (!strict && target is ContentControl)
                {
                    var child = ((ContentControl)target).Content as DependencyObject;
                    if (child != null)
                    {
                        yield return child;
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(target, i);
                }
            }
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualChildrenAndSelfIterator(element).Skip(1);
        }
        
        public static IEnumerable<DependencyObject> GetVisualChildrenAndSelf(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualChildrenAndSelfIterator(element);
        }

        private static IEnumerable<DependencyObject> GetVisualChildrenAndSelfIterator(this DependencyObject element)
        {
            Debug.Assert(element != null, "element should not be null!");

            yield return element;

            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                yield return VisualTreeHelper.GetChild(element, i);
            }
        }
    }
}
