using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;
using WpfSettings.Utils;

namespace WpfSettings.Example
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public ICommand ShowSettingsCommand { get; set; }
        public QuickSettings QuickSettings { get; set; }
        public GlobalSettings Settings { get; set; }
        public string WindowTitle { get; set; }

        public MainViewModel()
        {
            Settings = new GlobalSettings();
            QuickSettings = new QuickSettings(Settings);
            ShowSettingsCommand = new RelayCommand<MainWindow>(ShowSettings);
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

        private void ShowSettings(MainWindow mainWindow)
        {
            SettingsWindow settingsWindow = new SettingsWindow(Settings)
            {
                IconSource = "Resources.icon-cog.png",
                Owner = mainWindow
            };
            settingsWindow.ShowDialog();
        }
    }
}
