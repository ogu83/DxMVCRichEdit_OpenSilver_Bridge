using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DxMVCRichEdit_OpenSilver_Bridge
{
    public static class VisualTreeExtensions
    {
        internal static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }

        public static T GetChildInTreeOfType<T>(this FrameworkElement parent) where T : class
        {
            if (parent == null)
            {
                return null;
            }

            var queue = new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());
            while (queue.Count > 0)
            {
                FrameworkElement element = queue.Dequeue();
                if (element as T != null)
                {
                    return element as T;
                }

                foreach (FrameworkElement visualChild in (element).GetVisualChildren())
                {
                    queue.Enqueue(visualChild);
                }
            }
            return null;
        }
    }
}