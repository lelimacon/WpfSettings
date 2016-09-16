using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfSettingsControl
{
    /// <summary>
    ///     Interaction logic for WpfSettingsControl.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public ObservableCollection<object> ConfigElements { get; private set; }

        public SettingsPage()
        {
            ConfigElements = new ObservableCollection<object>();
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ConfigElement config;
            //ConfigElements.Add(new TitleConfig("test0", "My title"));
            ConfigElements.Add(new StringConfig("test1", "My string"));
            ConfigElements.Add(new TextConfig("test1", "My multiple line answer"));
            ConfigElements.Add(new BoolConfig("test2", "My checkbox"));
            ConfigElements.Add(new BoolConfig("test2", "My second checkbox"));
            //config = new TitleConfig("test0", "A title with details");
            //config.Details = "This part deals with other kinds of issues";
            //ConfigElements.Add(config);
            ConfigElements.Add(new BoolConfig("test2", "A checkbox on\ntwo lines"));
            config = new BoolConfig("test2", "Checkbox with details");
            config.Details = "You can either activate or deactivate this checkbox. It's made for it! By default it's deactivated so don't worry and be happy. We will take care of everything.";
            ConfigElements.Add(config);
            ConfigElements.Add(new ChoiceConfig("test2", "Select the value",
                new ObservableCollection<string> { "First Value", "Second Value" }));
        }
    }
}
