using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfSettings.Annotations;
using WpfSettingsControl;

namespace WpfSettings
{
    public class SettingsWindowViewModel : INotifyPropertyChanged
    {

        public object ExternalConfig { get; set; }
        public ObservableCollection<ConfigSection> InternalConfig { get; set; }
        public string MyNewText { get; set; }

        public ICommand ApplyCommand => new RelayCommand(o => Apply());
        public ICommand OkCommand => new RelayCommand(o => Ok());
        public ICommand CancelCommand => new RelayCommand(o => Cancel());

        public SettingsWindowViewModel()
        {
            var config = new ConfigManager(ExternalConfig);
            InternalConfig = config.ConvertConfig();
            MyNewText = "test";
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
