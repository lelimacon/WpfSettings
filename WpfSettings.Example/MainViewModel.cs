using PropertyChanged;
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
        public string WindowTitle => $"{Settings?.User?.Name}'s note";

        public MainViewModel()
        {
            Settings = new GlobalSettings();
            QuickSettings = new QuickSettings(Settings);
            ShowSettingsCommand = new RelayCommand<MainWindow>(ShowSettings);
            Settings.User.ReflectPropertyChanged(this,
                n => n == "Name" ? nameof(WindowTitle) : null);
        }

        private void ShowSettings(MainWindow mainWindow)
        {
            var settingsWindow = new SettingsWindow(Settings)
            {
                IconSource = "Resources.icon-cog.png",
                ExplorerWidth = 180,
                Width = 600,
                Height = 400,
                Owner = mainWindow
            };
            settingsWindow.ShowDialog();
        }
    }
}
