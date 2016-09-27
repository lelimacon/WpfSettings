using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.Utils;

namespace WpfSettings.Controls
{
    internal partial class SettingsExplorer : UserControl
    {
        public static readonly DependencyProperty ItemsProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>();

        public ObservableCollection<ConfigSection> Items
        {
            get { return (ObservableCollection<ConfigSection>) GetValue(ItemsProperty); }
            set { SetValueDp(ItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>();

        public string Selected
        {
            get { return (string) GetValue(SelectedProperty); }
            set { SetValueDp(SelectedProperty, value); }
        }

        public static readonly DependencyProperty ChangeActionProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>();

        public Action<ConfigSection> ChangeAction
        {
            get { return (Action<ConfigSection>)GetValue(ChangeActionProperty); }
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
            ConfigSection section = (ConfigSection) e.NewValue;
            ChangeAction?.Invoke(section);
        }
    }
}
