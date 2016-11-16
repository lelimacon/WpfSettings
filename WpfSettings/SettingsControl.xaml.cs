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
    public partial class SettingsControl : UserControl
    {
        public static readonly DependencyProperty SettingsProperty =
            MvvmUtils.RegisterDp<SettingsControl>(SettingsChanged);

        public static readonly DependencyProperty SettingElementsProperty =
            MvvmUtils.RegisterDp<SettingsControl>();

        public static readonly DependencyProperty AutoSaveProperty =
            MvvmUtils.RegisterDp<SettingsControl>();

        public static readonly DependencyProperty SaveProperty =
            MvvmUtils.RegisterDp<SettingsControl>();

        public static readonly DependencyProperty CurrentPageConfigProperty =
            MvvmUtils.RegisterDp<SettingsControl>();

        public static readonly DependencyProperty CategoryTitleProperty =
            MvvmUtils.RegisterDp<SettingsControl>();

        public static readonly DependencyProperty ExplorerWidthProperty =
            MvvmUtils.RegisterDp<SettingsControl>(defaultValue: "220");

        public object Settings
        {
            get { return GetValue(SettingsProperty); }
            set { SetValueDp(SettingsProperty, value); }
        }

        public ObservableCollection<SettingSection> SettingElements
        {
            get { return (ObservableCollection<SettingSection>) GetValue(SettingElementsProperty); }
            set { SetValueDp(SettingElementsProperty, value); }
        }

        public bool AutoSave
        {
            get { return (bool) GetValue(AutoSaveProperty); }
            set { SetValueDp(AutoSaveProperty, value); }
        }

        public Action Save
        {
            get { return (Action) GetValue(SaveProperty); }
            set { SetValueDp(SaveProperty, value); }
        }

        public ObservableCollection<SettingPageElement> CurrentPageConfig
        {
            get { return (ObservableCollection<SettingPageElement>) GetValue(CurrentPageConfigProperty); }
            set { SetValueDp(CurrentPageConfigProperty, value); }
        }

        public string CategoryTitle
        {
            get { return (string) GetValue(CategoryTitleProperty); }
            set { SetValueDp(CategoryTitleProperty, value); }
        }

        public string ExplorerWidth
        {
            get { return (string) GetValue(ExplorerWidthProperty); }
            set { SetValueDp(ExplorerWidthProperty, value); }
        }

        public Action<SettingSection> ChangeSectionAction { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsControl()
        {
            ChangeSectionAction = ChangeSection;
            InitializeComponent();
        }

        private void ChangeSection(SettingSection section)
        {
            CategoryTitle = section.Label;
            CurrentPageConfig = section.Elements;
        }

        private static void SettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsControl control = (SettingsControl) d;
            ConverterArgs args = new ConverterArgs {AutoSave = control.AutoSave};
            var sections = SettingsConverter.GetSections(control.Settings, args);
            control.SettingElements = sections;
            control.ChangeSection(control.SettingElements[0]);
            control.Save = control.SaveAll;
        }

        private void SaveAll()
        {
            foreach (SettingSection section in SettingElements)
                section.Save();
        }
    }
}
