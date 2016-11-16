using System.Drawing;
using System.Windows;
using WpfSettings.Utils;
using WpfSettings.Utils.Reflection;
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

        /// <summary>
        ///     Gets or sets the object used as base for settings.
        /// </summary>
        public object Settings
        {
            get { return ViewModel.Settings; }
            set { ViewModel.Settings = value; }
        }

        /// <summary>
        ///     Gets or sets the width of the explorer (left panel ListView).
        ///     Supports 'Auto', star or absolute values.
        ///     Defaults to 220.
        /// </summary>
        public string ExplorerWidth
        {
            get { return ViewModel.ExplorerWidth; }
            set { ViewModel.ExplorerWidth = value; }
        }

        public SettingsWindow()
        {
            ViewModel = new SettingsWindowViewModel();
            InitializeComponent();
        }
    }
}
