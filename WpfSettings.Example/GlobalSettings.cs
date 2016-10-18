using PropertyChanged;

namespace WpfSettings.Example
{
    [ImplementPropertyChanged]
    public class QuickSettings
    {
        private readonly GlobalSettings _settings;

        [SettingChoice("Gender",
             LabelWidth = 80,
             Details = "This will change the background to match your color preferences! No questions.")]
        public UserSettings.EGender Gender
        {
            get { return _settings.User.Gender; }
            set { _settings.User.Gender = value; }
        }

        [SettingString("Name", LabelWidth = 80)]
        public string Name
        {
            get { return _settings.User.Name; }
            set { _settings.User.Name = value; }
        }

        public QuickSettings(GlobalSettings settings)
        {
            _settings = settings;
            settings.User.ReflectPropertyChanged(this);
        }
    }

    public class GlobalSettings
    {
        [SettingSection("General",
             LabelWidth = 100,
             Icon = "Resources.icon-bulb.png")]
        public GeneralSettings General { get; }

        [SettingSection("Profile",
             LabelWidth = 100,
             Icon = "Resources.icon-skull.png")]
        public UserSettings User { get; }

        [SettingSection("Interface",
             LabelWidth = 100,
             Expansion = SectionExpansion.Expanded,
             Icon = "Resources.icon-window-system.png")]
        public InterfaceSettings Interface { get; }

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
        [SettingBool("Launch this program with Windows start-up",
             Details = "This options doesn't actually have any effect, don't worry.")]
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

        [SettingChoice("Gender",
             Details = "This will change the background to match your color preferences! No questions.")]
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
        [SettingSection("Style",
             Icon = "Resources.icon-flask.png")]
        public StyleSettings Style { get; set; }

        [SettingSection("Content",
             Icon = "Resources.icon-gift.png")]
        public ContentSettings Content { get; set; }

        public InterfaceSettings()
        {
            Style = new StyleSettings();
            Content = new ContentSettings();
        }
    }

    [ImplementPropertyChanged]
    public class BoxStyle
    {
        public enum TextStyle
        {
            [SettingField("Passive")] Normal,
            [SettingField("Agressive")] Bold,
            [SettingField("Discrete")] Italic
        }

        // TODO: color picker
        [SettingString(0, "Color")]
        public string Color { get; set; }

        [SettingChoice(1, "Style", Type = ChoiceType.RadioButtons)]
        public TextStyle Style { get; set; } = TextStyle.Bold;
    }

    [ImplementPropertyChanged]
    public class StyleSettings
    {
        [SettingGroup(1, "Title Style",
             LabelWidth = 70)]
        public BoxStyle TitleStyle { get; }

        [SettingGroup(2, "Content Style",
             LabelWidth = 70)]
        public BoxStyle ContentStyle { get; }

        public StyleSettings()
        {
            TitleStyle = new BoxStyle {Style = BoxStyle.TextStyle.Bold};
            ContentStyle = new BoxStyle {Style = BoxStyle.TextStyle.Normal};
        }
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
