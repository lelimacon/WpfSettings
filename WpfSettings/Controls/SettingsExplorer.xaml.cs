using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.Utils;

namespace WpfSettings.Controls
{
    internal partial class SettingsExplorer : UserControl
    {
        public static readonly DependencyProperty ItemsProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>();

        public ObservableCollection<ConfigSection> Items
        {
            get { return (ObservableCollection<ConfigSection>) GetValue(ItemsProperty); }
            set
            {
                //if (Items == null)
                    SetValueDp(ItemsProperty, value);
                //Items?.Clear();
                //foreach (var val in value)
                //    Items?.Add(val);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsExplorer()
        {
            InitializeComponent();
        }
    }
}
