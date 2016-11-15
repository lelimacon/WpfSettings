using System.Windows;

namespace WpfSettings.Example
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();
            InitializeComponent();
        }
    }
}
