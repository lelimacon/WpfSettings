using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.SettingElements;
using WpfSettings.Utils.Wpf;

namespace WpfSettings
{
    public partial class SettingsPage : UserControl
    {
        public static readonly DependencyProperty SettingsProperty =
            MvvmUtils.RegisterDp<SettingsPage>(SettingsChanged);

        public static readonly DependencyProperty SettingElementsProperty =
            MvvmUtils.RegisterDp<SettingsPage>();

        public static readonly DependencyProperty AutoSaveProperty =
            MvvmUtils.RegisterDp<SettingsPage>();

        public object Settings
        {
            get { return GetValue(SettingsProperty); }
            set { SetValueDp(SettingsProperty, value); }
        }

        public ObservableCollection<SettingPageElement> SettingElements
        {
            get { return (ObservableCollection<SettingPageElement>) GetValue(SettingElementsProperty); }
            set { SetValueDp(SettingElementsProperty, value); }
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
            if (DesignerProperties.GetIsInDesignMode(this))
                PreviewBlock.Visibility = Visibility.Visible;
            AutoSave = true;
        }

        private static void SettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsPage page = (SettingsPage) d;
            ConverterArgs args = new ConverterArgs {AutoSave = page.AutoSave};
            var elements = SettingsConverter.GetElements(page.Settings, args);
            page.SettingElements = elements;
        }
    }
}
