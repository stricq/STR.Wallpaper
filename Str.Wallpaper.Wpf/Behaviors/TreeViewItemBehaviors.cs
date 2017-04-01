using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using GongSolutions.Wpf.DragDrop.Utilities;


namespace Str.Wallpaper.Wpf.Behaviors {

  internal static class TreeViewItemBehaviors {

    #region BringIntoView

    public static readonly DependencyProperty BringIntoViewProperty = DependencyProperty.RegisterAttached("BringIntoView", typeof(bool), typeof(TreeViewItemBehaviors), new UIPropertyMetadata(false, onBringIntoViewChanged));

    public static bool GetBringIntoView(TreeViewItem treeViewItem) {
      return (bool)treeViewItem.GetValue(BringIntoViewProperty);
    }

    public static void SetBringIntoView(TreeViewItem treeViewItem, bool value) {
      treeViewItem.SetValue(BringIntoViewProperty, value);
    }

    private static void onBringIntoViewChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
      TreeViewItem item = o as TreeViewItem;

      if (item == null) return;

      if (e.NewValue is bool) {
        if ((bool)e.NewValue) item.Selected += onTreeViewItemSelected;
        else item.Selected -= onTreeViewItemSelected;
      }
    }

    private static void onTreeViewItemSelected(object sender, RoutedEventArgs e) {
      if (!ReferenceEquals(sender, e.OriginalSource)) {
        return;
      }

      TreeViewItem item = e.OriginalSource as TreeViewItem;

      if (item != null && item.IsSelected) {
        item.BringIntoView();
      }
    }

    #endregion BringIntoView

    #region PreventHorizontalScroll Property

    public static readonly DependencyProperty PreventHorizontalScrollProperty = DependencyProperty.RegisterAttached("PreventHorizontalScroll", typeof(bool), typeof(TreeViewItemBehaviors), new PropertyMetadata(false, onPreventHorizontalScrollChanged));

    public static bool GetPreventHorizontalScroll(TreeView treeView) {
      return (bool)treeView.GetValue(PreventHorizontalScrollProperty);
    }

    public static void SetPreventHorizontalScroll(TreeView treeView, bool value) {
      treeView.SetValue(PreventHorizontalScrollProperty, value);
    }

    private static void onPreventHorizontalScrollChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
      TreeView tree = o as TreeView;

      if (tree != null) {
        if ((bool)e.NewValue) {
          tree.SelectedItemChanged += onSelectedItemChanged;
        }
        else {
          tree.SelectedItemChanged -= onSelectedItemChanged;
        }
      }
    }

    private static void onSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
      if (!ReferenceEquals(sender, e.OriginalSource)) return;

      TreeView obj = sender as TreeView;

      ScrollViewer scrollViewer = obj?.GetVisualDescendents<ScrollViewer>().FirstOrDefault();

      if (scrollViewer != null) {
        double offset = scrollViewer.HorizontalOffset;

        ScrollBar scrollBar = scrollViewer.GetVisualDescendents<ScrollBar>().FirstOrDefault(sb => sb.Orientation == Orientation.Horizontal);

        if (scrollBar != null) {
          RoutedPropertyChangedEventHandler<double> handler = null;

          handler = (s, args) => {
            scrollBar.ValueChanged -= handler;

            scrollViewer.ScrollToHorizontalOffset(offset);
          };

          scrollBar.ValueChanged += handler;
        }
      }
    }

    #endregion PreventHorizontalScroll Property

  }

}
