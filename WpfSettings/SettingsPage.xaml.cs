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
    public partial class SettingsPage : UserControl
    {
        public static readonly DependencyProperty SettingsProperty =
            MvvmUtils.RegisterDp<SettingsPage>(SettingsChanged);

        public static readonly DependencyProperty SettingElementsProperty =
            MvvmUtils.RegisterDp<SettingsPage>(SettingsElementsChanged);

        public static readonly DependencyProperty AutoSaveProperty =
            MvvmUtils.RegisterDp<SettingsPage>(true);

        public static readonly DependencyProperty FilterProperty =
            MvvmUtils.RegisterDp<SettingsPage>();

        public string Filter
        {
            get => (string) GetValue(FilterProperty);
            set => SetValueDp(FilterProperty, value);
        }

        public object Settings
        {
            get => GetValue(SettingsProperty);
            set => SetValueDp(SettingsProperty, value);
        }

        public ObservableCollection<SettingPageElement> SettingElements
        {
            get => (ObservableCollection<SettingPageElement>) GetValue(SettingElementsProperty);
            set => SetValueDp(SettingElementsProperty, value);
        }

        public bool AutoSave
        {
            get => (bool) GetValue(AutoSaveProperty);
            set => SetValueDp(AutoSaveProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsPage()
        {
            InitializeComponent();

            // Show preview for design mode
            if (DesignerProperties.GetIsInDesignMode(this))
                PreviewBlock.Visibility = Visibility.Visible;
        }

        private static void SettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsPage page = (SettingsPage) d;
            ConverterArgs args = new ConverterArgs {AutoSave = page.AutoSave};
            var elements = SettingsConverter.GetElements(page.Settings, args);
            page.SettingElements = elements;
            //page.AddResources();
        }
        
        private static void SettingsElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsPage page = (SettingsPage) d;
            page.AddResources();
        }

        private void AddResources()
        {
            if (SettingElements == null)
                return;
            foreach (SettingPageElement element in SettingElements)
            {
                var rd = new ResourceDictionary {Source = new Uri(element.ResourceUri)};
                Resources.MergedDictionaries.Add(rd);
            }
        }
    }
}
