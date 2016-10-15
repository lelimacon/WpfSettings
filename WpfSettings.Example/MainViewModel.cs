using PropertyChanged;
using System.ComponentModel;
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
        public string WindowTitle { get; set; }

        #endregion Properties

        public MainViewModel()
        {
            Settings = new GlobalSettings();
            SetTitle();
            dynamic userSettings = Settings.User;
            userSettings.PropertyChanged += new PropertyChangedEventHandler(SettingChanged);
        }

        private void SettingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                SetTitle();
        }

        private void SetTitle()
        {
            WindowTitle = $"{Settings?.User?.Name}'s note";
        }

        private void ShowSettings()
        {
            SettingsWindow window = new SettingsWindow(Settings);
            window.IconSource = "Resources.icon-cog.png";
            window.ShowDialog();
        }
    }
}
