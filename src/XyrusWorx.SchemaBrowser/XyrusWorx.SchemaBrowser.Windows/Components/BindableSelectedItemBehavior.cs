using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace XyrusWorx.SchemaBrowser.Windows.Components
{
    class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));
        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();
            
            if (AssociatedObject == null) return;
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null) return;
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is TreeViewItem item)) return;
            item.SetValue(TreeViewItem.IsSelectedProperty, true);
        }
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }
    }
}