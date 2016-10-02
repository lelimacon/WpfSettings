using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfSettings.Annotations;
using WpfSettings.Utils;

namespace WpfSettings.Example
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MyConfig _myConfig;

        #region Properties

        public ICommand ShowSettingsCommand => new RelayCommand(ShowSettings);

        public MyConfig MyConfig
        {
            get { return _myConfig; }
            set
            {
                _myConfig = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Properties

        public MainViewModel()
        {
            MyConfig = new MyConfig();
        }

        private void ShowSettings()
        {
            SettingsWindow window = new SettingsWindow(MyConfig);
            window.ShowDialog();
        }
    }
}
