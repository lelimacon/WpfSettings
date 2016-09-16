using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WpfSettingsControl
{
    /// <summary>
    /// Interaction logic for SettingsExplorer.xaml
    /// </summary>
    public partial class SettingsExplorer : UserControl
    {
        //public ObservableCollection<ConfigSection> Items { get; set; }

        public ObservableCollection<ConfigSection> Itemss
        {
            get { return (ObservableCollection<ConfigSection>)GetValue(ItemsProperty); }
            set
            {
                if (Itemss == null)
                    SetValueDp(ItemsProperty, value);
                var items = Itemss;
                items.Clear();
                foreach (var val in value)
                    items.Add(val);
                //SetValueDp(ItemsProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Itemss),
                typeof(ObservableCollection<ConfigSection>),
                typeof(SettingsExplorer), null);

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsExplorer()
        {
            //Items = new ObservableCollection<ConfigSection>();
            InitializeComponent();
            (Content as FrameworkElement).DataContext = this;

            /*
            var section = new ConfigSection("general", "General");
            //section.Image = "icon-search.png";
            Items.Add(section);
            section = new ConfigSection("env", "Environment");
            section.SubSections.Add(new ConfigSection("test1", "General"));
            section.SubSections.Add(new ConfigSection("test2", "Documents"));
            Items.Add(section);
            */
        }
    }
}
