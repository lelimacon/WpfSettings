using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfSettings.Annotations;
using WpfSettings.Utils;

namespace WpfSettings.Example
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Settings _settings;

        #region Properties

        public ICommand ShowSettingsCommand => new RelayCommand(ShowSettings);

        public Settings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Properties

        public MainViewModel()
        {
            Settings = new Settings();
        }

        private void ShowSettings()
        {
            SettingsWindow window = new SettingsWindow(Settings);
            window.ShowDialog();
        }
    }
}
