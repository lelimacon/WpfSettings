using System.Collections.ObjectModel;
using System.Windows;

namespace WpfSettingsControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public object ExternalConfig { get; set; }
        public ObservableCollection<ConfigSection> InternalConfig { get; set; }

        public SettingsWindow()
        {
            var config = new ConfigManager(ExternalConfig);
            InternalConfig = config.ConvertConfig();
            InitializeComponent();
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
        }
    }
}
