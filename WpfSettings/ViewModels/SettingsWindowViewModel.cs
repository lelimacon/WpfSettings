using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfSettings.Annotations;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.ViewModels
{
    public class SettingsWindowViewModel : INotifyPropertyChanged
    {
        private string _explorerWidth;
        private object _settings;
        private int _unsavedSettings;

        public object Settings
        {
            get { return _settings; }
            set { Set(ref _settings, value); }
        }

        public string ExplorerWidth
        {
            get { return _explorerWidth; }
            set { Set(ref _explorerWidth, value); }
        }

        public int UnsavedSettings
        {
            get { return _unsavedSettings; }
            set { Set(ref _unsavedSettings, value); }
        }

        public ICommand ApplyCommand => new RelayCommand<SettingsWindow>(Apply);
        public ICommand OkCommand => new RelayCommand<SettingsWindow>(Ok);
        public ICommand CancelCommand => new RelayCommand<SettingsWindow>(Cancel);

        public Action Save { private get; set; }

        protected void Set<T>(ref T variable, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, variable))
                return;
            variable = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Apply(SettingsWindow window)
        {
            Save();
        }

        private void Ok(SettingsWindow window)
        {
            // Changes the focus so the current setting (if changed) is saved...
            // And thus even if it doesn't have an UpdateSourceTrigger on PropertyChanged.
            window.OkButton.Focus();
            Save();
            window.Close();
        }

        private void Cancel(SettingsWindow window)
        {
            window.Close();
        }
    }
}
