using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfSettings.Annotations;

namespace WpfSettings.Example
{
    public class GlobalSettings
    {
        [SettingSection("General")]
        public GeneralSettings General { get; set; }

        [SettingSection("Profile")]
        public UserSettings User { get; set; }

        [SettingSection("Interface")]
        public InterfaceSettings Interface { get; set; }

        public GlobalSettings()
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

    public class UserSettings : Notify
    {
        private EGender _gender = EGender.Other;
        private string _name = "Bob";
        private string _age;

        public enum EGender
        {
            [SettingField("Male")] Male,
            [SettingField("Female")] Female,
            [SettingField("Other")] Other
        }

        [SettingChoice("Gender")]
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

    public class InterfaceSettings : Notify
    {
        [SettingSection("Style")]
        public StyleSettings Style { get; set; }

        [SettingSection("Content")]
        public ContentSettings Content { get; set; }

        public InterfaceSettings()
        {
            Style = new StyleSettings();
            Content = new ContentSettings();
        }
    }

    public class StyleSettings : Notify
    {
        private string _bgColor;
        private string _textColor;
        private TextStyle _titleStyle = TextStyle.Bold;
        private TextStyle _contentStyle = TextStyle.Italic;

        public enum TextStyle
        {
            [SettingField("Passive")] Normal,
            [SettingField("Agressive")] Bold,
            [SettingField("Discrete")] Italic
        }

        // TODO: color picker
        [SettingString(0, "Title text color")]
        public string TitleColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                OnPropertyChanged();
            }
        }

        [SettingString(2, "Content text color")]
        public string ContentColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                OnPropertyChanged();
            }
        }

        [SettingChoice(1, "Title text style", Type = ChoiceType.RadioButtons)]
        public TextStyle TitleStyle
        {
            get { return _titleStyle; }
            set
            {
                _titleStyle = value;
                OnPropertyChanged();
            }
        }

        [SettingChoice(3, "Content text style", Type = ChoiceType.RadioButtons)]
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

    public class ContentSettings : Notify
    {
        private string _title = "My Super Note!";
        private string _pageContent = "My Super Content!";

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
