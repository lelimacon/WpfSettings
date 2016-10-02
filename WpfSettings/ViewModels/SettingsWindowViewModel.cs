using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public Action<ConfigSection> ChangeSectionAction { get; set; }

        public ICommand ApplyCommand => new RelayCommand(Apply);
        public ICommand OkCommand => new RelayCommand(Ok);
        public ICommand CancelCommand => new RelayCommand(Cancel);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public SettingsWindowViewModel()
        {
            ExternalConfig = new MyConfig();
            var configManager = new ConfigManager(ExternalConfig);
            InternalConfig = configManager.ConvertConfig();
            CurrentPageConfig = InternalConfig[0].Elements;
            ChangeSectionAction = ChangeSection;
        }

        private void ChangeSection(ConfigSection section)
        {
            CurrentPageConfig = section.Elements;
        }

        private void Apply()
        {
        }

        private void Ok()
        {
        }

        private void Cancel()
        {
        }
    }
}
