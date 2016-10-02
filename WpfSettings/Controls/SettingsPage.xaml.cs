using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.Config;
using WpfSettings.Utils;

namespace WpfSettings.Controls
{
    internal partial class SettingsPage : UserControl
    {
        public static readonly DependencyProperty ConfigElementsProperty =
            MvvmUtils.RegisterDp<SettingsPage>();

        public ObservableCollection<ConfigPageElement> ConfigElements
        {
            get { return (ObservableCollection<ConfigPageElement>)GetValue(ConfigElementsProperty); }
            set { SetValueDp(ConfigElementsProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsPage()
        {
            InitializeComponent();
        }
    }
}
