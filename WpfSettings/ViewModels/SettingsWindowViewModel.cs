using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfSettings.Annotations;
using WpfSettings.Config;
using WpfSettings.Utils;

namespace WpfSettings.ViewModels
{
    public class SettingsWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ConfigPageElement> _currentPageConfig;
        private ObservableCollection<ConfigSection> _internalConfig;
        private string _categoryTitle;
        public object ExternalConfig { get; set; }

        public ObservableCollection<ConfigSection> InternalConfig
        {
            get { return _internalConfig; }
            set
            {
                _internalConfig = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ConfigPageElement> CurrentPageConfig
        {
            get { return _currentPageConfig; }
            set
            {
                _currentPageConfig = value;
                OnPropertyChanged();
            }
        }

        public string CategoryTitle
        {
            get { return _categoryTitle; }
            set
            {
                _categoryTitle = value;
                OnPropertyChanged();
            }
        }

        public Action<ConfigSection> ChangeSectionAction { get; set; }

        public ICommand ApplyCommand => new RelayCommand<SettingsWindow>(Apply);
        public ICommand OkCommand => new RelayCommand<SettingsWindow>(Ok);
        public ICommand CancelCommand => new RelayCommand<SettingsWindow>(Cancel);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public SettingsWindowViewModel(object config)
        {
            ExternalConfig = config;
            ConverterArgs args = new ConverterArgs {AutoSave = false};
            var sections = SettingsConverter.GetSections(config, args);
            InternalConfig = sections;
            ChangeSectionAction = ChangeSection;
            ChangeSection(InternalConfig.First());
        }

        private void ChangeSection(ConfigSection section)
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
            foreach (ConfigSection section in InternalConfig)
                section.Save();
        }
    }
}
