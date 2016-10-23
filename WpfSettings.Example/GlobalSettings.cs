using PropertyChanged;
using System;

namespace WpfSettings.Example
{
    [ImplementPropertyChanged]
    public class QuickSettings
    {
        private readonly GlobalSettings _settings;

        [SettingChoice(LabelWidth = 80,
             Details = "This will change the background to match your color preferences! No questions.")]
        public UserSettings.EGender Gender
        {
            get { return _settings.User.Gender; }
            set { _settings.User.Gender = value; }
        }

        [SettingString(LabelWidth = 80)]
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
        [SettingSection(
             LabelWidth = 100,
             Icon = "Resources.icon-bulb.png")]
        public GeneralSettings General { get; }

        [SettingSection(
             LabelWidth = 100,
             Icon = "Resources.icon-skull.png")]
        public UserSettings User { get; }

        [SettingSection(
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
        [SettingBool(Label = "Launch this program with Windows start-up",
             Details = "This option doesn't actually have any effect, don't worry.")]
        public bool StartReboot { get; set; }
    }

    [ImplementPropertyChanged]
    public class UserSettings
    {
        public enum EGender
        {
            [SettingField] Male,
            [SettingField] Female,
            [SettingField] Other
        }

        [SettingChoice(
             Details = "This will change the background to match your color preferences! No questions.")]
        public EGender Gender { get; set; } = EGender.Other;

        [SettingString]
        public string Name { get; set; } = "Bob";

        // TODO: int selection
        [SettingString]
        public string Age { get; set; }
    }

    [ImplementPropertyChanged]
    public class InterfaceSettings
    {
        [SettingSection(Icon = "Resources.icon-flask.png")]
        public StyleSettings Style { get; set; }

        [SettingSection(Icon = "Resources.icon-gift.png")]
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
            [SettingField(Label = "Passive")] Normal,
            [SettingField(Label = "Aggressive")] Bold,
            [SettingField(Label = "Discrete")] Italic
        }

        // TODO: color picker
        [SettingString(Position = 0)]
        public string Color { get; set; }

        [SettingChoice(Position = 1, Type = ChoiceType.RadioButtons)]
        public TextStyle Style { get; set; } = TextStyle.Bold;
    }

    [ImplementPropertyChanged]
    public class StyleSettings
    {
        [SettingGroup(Position = 2, LabelWidth = 70)]
        public BoxStyle ContentStyle { get; }

        [SettingGroup(Position = 1, LabelWidth = 70)]
        public BoxStyle TitleStyle { get; }

        public StyleSettings()
        {
            TitleStyle = new BoxStyle {Style = BoxStyle.TextStyle.Bold};
            ContentStyle = new BoxStyle {Style = BoxStyle.TextStyle.Normal};
        }
    }

    [ImplementPropertyChanged]
    public class ContentSettings
    {
        private static readonly Random Rand = new Random();

        private readonly string[] _endings =
        {
            "Sugar is sweet,\nAnd so are you.",
            "I like donuts.\nDonuts are good.",
            "I want to play video games with you",
            "Cows are moo,\nWhat about you?",
            "Vodka is cheaper\nThan dinner for two.",
            "God made me pretty.\nWhat happen to you?",
            "No matter how stupid,\nI still want a friend like you :)",
            "Let's make purple together."
        };

        [SettingString]
        public string Title { get; set; } = "My Poetry";

        [SettingText(Height = 80)]
        public string PageContent { get; set; }

        [SettingButton(Label = "Randomize content")]
        public Action Randomize { get; set; }

        public ContentSettings()
        {
            Randomize = ChangeContent;
            ChangeContent();
        }

        private void ChangeContent()
        {
            PageContent = "Roses are red,\nViolets are blue,\n" + GetRand(_endings);
        }

        private static T GetRand<T>(T[] array)
        {
            return array[Rand.Next(array.Length)];
        }
    }
}
