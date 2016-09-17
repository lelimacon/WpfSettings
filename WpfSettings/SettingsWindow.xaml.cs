using System.Windows;

namespace WpfSettings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
