using System.Drawing;
using System.Windows;
using WpfSettings.Utils;
using WpfSettings.ViewModels;

namespace WpfSettings
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindowViewModel ViewModel { get; set; }

        /// <summary>
        ///     Sets the Window icon from the specified resource file.
        /// </summary>
        public string IconSource
        {
            set
            {
                var stream = ResourceUtils.FromParentAssembly(value);
                var image = new Bitmap(stream);
                Icon = image.ToBitmapSource();
            }
        }

        public int ExplorerWidth
        {
            get { return ViewModel.ExplorerWidth; }
            set { ViewModel.ExplorerWidth = value; }
        }

        public SettingsWindow(object config)
        {
            ViewModel = new SettingsWindowViewModel(config);
            InitializeComponent();
        }
    }
}
