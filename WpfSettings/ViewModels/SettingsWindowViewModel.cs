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
        public object ExternalConfig { get; set; }
        public ObservableCollection<ConfigSection> InternalConfig { get; set; }
        public ObservableCollection<ConfigPageElement> CurrentPageConfig { get; set; }

        public ICommand ApplyCommand => new RelayCommand(Apply);
        public ICommand OkCommand => new RelayCommand(Ok);
        public ICommand CancelCommand => new RelayCommand(Cancel);

        public SettingsWindowViewModel()
        {
            var configManager = new ConfigManager(ExternalConfig);
            InternalConfig = configManager.ConvertConfig();
            CurrentPageConfig = InternalConfig[0].Elements;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
