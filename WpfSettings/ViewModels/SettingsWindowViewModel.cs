using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfSettings.Annotations;
using WpfSettings.SettingElements;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.ViewModels
{
    public class SettingsWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SettingPageElement> _currentPageConfig;
        private ObservableCollection<SettingSection> _internalConfig;
        private string _categoryTitle;
        private int _explorerWidth;
        public object ExternalConfig { get; set; }

        public ObservableCollection<SettingSection> InternalConfig
        {
            get { return _internalConfig; }
            set { Set(ref _internalConfig, value); }
        }

        public ObservableCollection<SettingPageElement> CurrentPageConfig
        {
            get { return _currentPageConfig; }
            set { Set(ref _currentPageConfig, value); }
        }

        public string CategoryTitle
        {
            get { return _categoryTitle; }
            set { Set(ref _categoryTitle, value); }
        }

        public int ExplorerWidth
        {
            get { return _explorerWidth; }
            set { Set(ref _explorerWidth, value); }
        }

        public Action<SettingSection> ChangeSectionAction { get; set; }

        public ICommand ApplyCommand => new RelayCommand<SettingsWindow>(Apply);
        public ICommand OkCommand => new RelayCommand<SettingsWindow>(Ok);
        public ICommand CancelCommand => new RelayCommand<SettingsWindow>(Cancel);


        protected void Set<T>(ref T variable, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, variable))
                return;
            variable = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public SettingsWindowViewModel(object config)
        {
            ExternalConfig = config;
            ExplorerWidth = 220;
            ConverterArgs args = new ConverterArgs {AutoSave = false};
            var sections = SettingsConverter.GetSections(config, args);
            InternalConfig = sections;
            ChangeSectionAction = ChangeSection;
            ChangeSection(InternalConfig.First());
        }

        private void ChangeSection(SettingSection section)
        {
            CategoryTitle = section.Label;
            CurrentPageConfig = section.Elements;
        }

        private void Apply(SettingsWindow window)
        {
            Save();
        }

        private void Ok(SettingsWindow window)
        {
            // Changes the focus so the current setting (if changed) is saved
            window.OkButton.Focus();
            Save();
            window.Close();
        }

        private void Cancel(SettingsWindow window)
        {
            window.Close();
        }

        private void Save()
        {
            foreach (SettingSection section in InternalConfig)
                section.Save();
        }
    }
}
