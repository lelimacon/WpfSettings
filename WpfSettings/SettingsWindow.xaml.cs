using System.Windows;
using WpfSettings.ViewModels;

namespace WpfSettings
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindowViewModel ViewModel { get; set; }

        public SettingsWindow()
        {
            ViewModel = new SettingsWindowViewModel();
            InitializeComponent();
        }
    }
}
