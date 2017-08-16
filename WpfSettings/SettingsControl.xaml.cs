using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        public static readonly DependencyProperty FilterProperty =
            MvvmUtils.RegisterDp<SettingsControl>();

        public static readonly DependencyProperty UnsavedSettingsProperty =
            MvvmUtils.RegisterDp<SettingsControl>(new FrameworkPropertyMetadata {BindsTwoWayByDefault = false});

        public object Settings
        {
            get => GetValue(SettingsProperty);
            set => SetValueDp(SettingsProperty, value);
        }

        public ObservableCollection<SettingSection> SettingElements
        {
            get => (ObservableCollection<SettingSection>) GetValue(SettingElementsProperty);
            set => SetValueDp(SettingElementsProperty, value);
        }

        public bool AutoSave
        {
            get => (bool) GetValue(AutoSaveProperty);
            set => SetValueDp(AutoSaveProperty, value);
        }

        public Action Save
        {
            get => (Action) GetValue(SaveProperty);
            set => SetValueDp(SaveProperty, value);
        }

        public ObservableCollection<SettingPageElement> CurrentPageConfig
        {
            get => (ObservableCollection<SettingPageElement>) GetValue(CurrentPageConfigProperty);
            set => SetValueDp(CurrentPageConfigProperty, value);
        }

        public string CategoryTitle
        {
            get => (string) GetValue(CategoryTitleProperty);
            set => SetValueDp(CategoryTitleProperty, value);
        }

        public string ExplorerWidth
        {
            get => (string) GetValue(ExplorerWidthProperty);
            set => SetValueDp(ExplorerWidthProperty, value);
        }

        public string Filter
        {
            get => (string) GetValue(FilterProperty);
            set => SetValueDp(FilterProperty, value);
        }

        public int UnsavedSettings
        {
            get => (int) GetValue(UnsavedSettingsProperty);
            set => SetValueDp(UnsavedSettingsProperty, value);
        }

        public Action<SettingSection> ChangeSectionAction { get; }

        public ICommand EmptySearchBoxCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsControl()
        {
            ChangeSectionAction = ChangeSection;
            EmptySearchBoxCommand = new RelayCommand(EmptySearchBox);
            InitializeComponent();
        }

        private void ChangeSection(SettingSection section)
        {
            CategoryTitle = section?.Label;
            CurrentPageConfig = section?.Elements;
        }

        private void EmptySearchBox()
        {
            Filter = string.Empty;
        }

        private static void SettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsControl control = (SettingsControl) d;
            ConverterArgs args = new ConverterArgs
            {
                AutoSave = control.AutoSave,
                SelectSection = control.Explorer.SelectSection
            };
            var sections = SettingsConverter.GetSections(control.Settings, args);
            control.SettingElements = sections;
            control.ChangeSection(control.SettingElements[0]);
            control.Save = control.SaveAll;
            foreach (var section in control.SettingElements)
                section.ValueChanged += control.RecalculateUnsaved;
        }

        private void RecalculateUnsaved(object sender, ValueChangedEventArgs args)
        {
            RecalculateUnsaved();
        }

        private void RecalculateUnsaved()
        {
            UnsavedSettings = SettingElements.Sum(s => s.UnsavedSettings);
        }

        private void SaveAll()
        {
            foreach (SettingSection section in SettingElements)
                section.Save();
            RecalculateUnsaved();
        }
    }
}
