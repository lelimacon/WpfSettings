using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.Config;
using WpfSettings.Utils;

namespace WpfSettings.Controls
{
    public partial class SettingsPage : UserControl
    {
        public static readonly DependencyProperty ConfigElementsProperty =
            MvvmUtils.RegisterDp<SettingsPage>();

        public static readonly DependencyProperty SettingsProperty =
            MvvmUtils.RegisterDp<SettingsPage>();

        public static readonly DependencyProperty AutoSaveProperty =
            MvvmUtils.RegisterDp<SettingsPage>();

        public ObservableCollection<ConfigPageElement> ConfigElements
        {
            get { return (ObservableCollection<ConfigPageElement>) GetValue(ConfigElementsProperty); }
            set { SetValueDp(ConfigElementsProperty, value); }
        }

        public object Settings
        {
            get { return GetValue(SettingsProperty); }
            set
            {
                SetValueDp(SettingsProperty, value);
                ConverterArgs args = new ConverterArgs {AutoSave = AutoSave};
                var elements = SettingsConverter.GetElements(value, args);
                ConfigElements = elements;
            }
        }

        public bool AutoSave
        {
            get { return (bool) GetValue(AutoSaveProperty); }
            set { SetValueDp(AutoSaveProperty, value); }
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
            AutoSave = true;
        }
    }
}
