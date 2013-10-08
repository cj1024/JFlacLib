using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CJToolkit.ViewTree
{
    public class ViewTreeHelper
    {

        #region 高级的方法，用于获取控件的子元素

        public static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
        {
            var visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, ICollection<T> visualCollection) where T : UIElement
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        public static VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            var groups = VisualStateManager.GetVisualStateGroups(element);
            return groups.Cast<VisualStateGroup>().FirstOrDefault(@group => @group.Name == name);
        }

        #endregion

    }
}
