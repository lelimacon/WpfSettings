using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.SettingElements;
using WpfSettings.Utils.Wpf;

namespace WpfSettings
{
    internal partial class SettingsExplorer : UserControl
    {
        public static readonly DependencyProperty ItemsProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>(ItemsChanged);

        public static readonly DependencyProperty ChangeActionProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>(new FrameworkPropertyMetadata());

        public ObservableCollection<SettingSection> Items
        {
            get { return (ObservableCollection<SettingSection>) GetValue(ItemsProperty); }
            set { SetValueDp(ItemsProperty, value); }
        }

        public Action<SettingSection> ChangeAction
        {
            get { return (Action<SettingSection>) GetValue(ChangeActionProperty); }
            set { SetValueDp(ChangeActionProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsExplorer()
        {
            InitializeComponent();
            ItemsTreeView.Focus();
        }

        private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SettingSection section = (SettingSection) e.NewValue;
            ChangeAction?.Invoke(section);
        }

        private static void ItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsExplorer explorer = (SettingsExplorer) d;
            if (explorer.Items.Any())
                explorer.SelectSection(0);
        }

        public void SelectSection(int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new IndexOutOfRangeException();
            Items[index].IsSelected = true;
            ItemsTreeView.Focus();
        }
    }
}
