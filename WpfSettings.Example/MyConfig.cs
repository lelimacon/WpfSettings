namespace WpfSettings.Example
{
    public class MyConfig
    {
        public GeneralSettings General { get; set; }
        public UserSettings User { get; set; }
        public InterfaceSettings Interface { get; set; }

        public MyConfig()
        {
            General = new GeneralSettings();
            User = new UserSettings();
            Interface = new InterfaceSettings();
        }
    }

    [SettingSection("General")]
    public class GeneralSettings
    {
        [SettingField("Launch this program with Windows start-up")]
        public bool StartReboot { get; set; }
    }

    [SettingSection("Profile")]
    public class UserSettings
    {
        public enum EGender
        {
            [SettingField("Male")] Male,
            [SettingField("Female")] Female,
            [SettingField("Other")] Other,
        }

        [SettingField("Gender")]
        public EGender Gender { get; set; }

        [SettingField("Name")]
        public string Name { get; set; }

        // TODO: int selection
        [SettingField("Age")]
        public string Age { get; set; }
    }

    [SettingSection("Interface")]
    public class InterfaceSettings
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
    public class StyleSettings
    {
        public enum TextStyle
        {
            [SettingField("Bold")] Bold,
            [SettingField("Italic")] Italic
        }

        // TODO: color picker
        [SettingField("Background color")]
        public string BgColor { get; set; }

        [SettingField("Text color")]
        public string TextColor { get; set; }

        [SettingField("Title text style")]
        public TextStyle TitleStyle { get; set; }

        [SettingField("Content text style")]
        public TextStyle ContentStyle { get; set; }
    }

    [SettingSection("Content")]
    public class ContentSettings
    {
        [SettingField("Title")]
        public string Title { get; set; }

        [SettingField("Page content")]
        public string PageContent { get; set; }
    }
}
