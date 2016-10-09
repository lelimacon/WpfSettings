using PropertyChanged;
using System.Windows.Input;
using WpfSettings.Utils;

namespace WpfSettings.Example
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        #region Properties

        public ICommand ShowSettingsCommand => new RelayCommand(ShowSettings);

        public GlobalSettings Settings { get; set; }

        #endregion Properties

        public MainViewModel()
        {
            Settings = new GlobalSettings();
        }

        private void ShowSettings()
        {
            SettingsWindow window = new SettingsWindow(Settings);
            window.ShowDialog();
        }
    }
}
