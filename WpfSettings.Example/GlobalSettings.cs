using PropertyChanged;

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

    [ImplementPropertyChanged]
    public class GeneralSettings
    {
        [SettingBool("Launch this program with Windows start-up")]
        public bool StartReboot { get; set; }
    }

    [ImplementPropertyChanged]
    public class UserSettings
    {
        public enum EGender
        {
            [SettingField("Male")] Male,
            [SettingField("Female")] Female,
            [SettingField("Other")] Other
        }

        [SettingChoice("Gender")]
        public EGender Gender { get; set; } = EGender.Other;

        [SettingString("Name")]
        public string Name { get; set; } = "Bob";

        // TODO: int selection
        [SettingString("Age")]
        public string Age { get; set; }
    }

    [ImplementPropertyChanged]
    public class InterfaceSettings
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

    [ImplementPropertyChanged]
    public class StyleSettings
    {
        public enum TextStyle
        {
            [SettingField("Passive")] Normal,
            [SettingField("Agressive")] Bold,
            [SettingField("Discrete")] Italic
        }

        // TODO: color picker
        [SettingString(0, "Title text color")]
        public string TitleColor { get; set; }

        [SettingString(2, "Content text color")]
        public string ContentColor { get; set; }

        [SettingChoice(1, "Title text style", Type = ChoiceType.RadioButtons)]
        public TextStyle TitleStyle { get; set; } = TextStyle.Bold;

        [SettingChoice(3, "Content text style", Type = ChoiceType.RadioButtons)]
        public TextStyle ContentStyle { get; set; } = TextStyle.Italic;
    }

    [ImplementPropertyChanged]
    public class ContentSettings
    {
        [SettingString("Title")]
        public string Title { get; set; } = "My Super Note!";

        [SettingText("Page content")]
        public string PageContent { get; set; } = "My Super Content!";
    }
}
