using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            MvvmUtils.RegisterDp<SettingsExplorer>();

        public static readonly DependencyProperty SelectedProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>();

        public static readonly DependencyProperty ChangeActionProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>();

        public ObservableCollection<SettingSection> Items
        {
            get { return (ObservableCollection<SettingSection>) GetValue(ItemsProperty); }
            set { SetValueDp(ItemsProperty, value); }
        }

        public string Selected
        {
            get { return (string) GetValue(SelectedProperty); }
            set { SetValueDp(SelectedProperty, value); }
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
        }

        private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SettingSection section = (SettingSection) e.NewValue;
            ChangeAction?.Invoke(section);
        }
    }
}
