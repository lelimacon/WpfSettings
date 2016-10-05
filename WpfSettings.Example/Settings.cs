using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfSettings.Annotations;

namespace WpfSettings.Example
{
    public class Settings
    {
        public GeneralSettings General { get; set; }
        public UserSettings User { get; set; }
        public InterfaceSettings Interface { get; set; }

        public Settings()
        {
            General = new GeneralSettings();
            User = new UserSettings();
            Interface = new InterfaceSettings();
        }
    }

    public class Notify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [SettingSection("General")]
    public class GeneralSettings : Notify
    {
        private bool _startReboot;

        [SettingBool("Launch this program with Windows start-up")]
        public bool StartReboot
        {
            get { return _startReboot; }
            set
            {
                _startReboot = value;
                OnPropertyChanged();
            }
        }
    }

    [SettingSection("Profile")]
    public class UserSettings : Notify
    {
        private EGender _gender;
        private string _name;
        private string _age;

        public enum EGender
        {
            [SettingField("Male")] Male,
            [SettingField("Female")] Female,
            [SettingField("Other")] Other
        }

        [SettingComboBox("Gender")]
        public EGender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        [SettingString("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        // TODO: int selection
        [SettingString("Age")]
        public string Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }
    }

    [SettingSection("Interface")]
    public class InterfaceSettings : Notify
    {
        public StyleSettings Style { get; set; }
        public ContentSettings Content { get; set; }

        public InterfaceSettings()
        {
            Style = new StyleSettings();
            Content = new ContentSettings();
        }
    }

    [SettingSection("Style")]
    public class StyleSettings : Notify
    {
        private string _bgColor;
        private string _textColor;
        private TextStyle _titleStyle;
        private TextStyle _contentStyle;

        public enum TextStyle
        {
            [SettingField("Bold")] Bold,
            [SettingField("Italic")] Italic
        }

        // TODO: color picker
        [SettingString("Background color")]
        public string BgColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                OnPropertyChanged();
            }
        }

        [SettingString("Text color")]
        public string TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                OnPropertyChanged();
            }
        }

        [SettingComboBox("Title text style")]
        public TextStyle TitleStyle
        {
            get { return _titleStyle; }
            set
            {
                _titleStyle = value;
                OnPropertyChanged();
            }
        }

        [SettingComboBox("Content text style")]
        public TextStyle ContentStyle
        {
            get { return _contentStyle; }
            set
            {
                _contentStyle = value;
                OnPropertyChanged();
            }
        }
    }

    [SettingSection("Content")]
    public class ContentSettings : Notify
    {
        private string _title;
        private string _pageContent;

        [SettingString("Title")]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        [SettingText("Page content")]
        public string PageContent
        {
            get { return _pageContent; }
            set
            {
                _pageContent = value;
                OnPropertyChanged();
            }
        }
    }
}
