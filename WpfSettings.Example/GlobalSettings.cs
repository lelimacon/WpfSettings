using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfSettings.SettingElements;

namespace WpfSettings.Example
{
    [ImplementPropertyChanged]
    public class QuickSettings
    {
        private readonly GlobalSettings _settings;

        [SettingChoice(LabelWidth = "80",
             Details = "This will change the background to match your color preferences! No questions.")]
        public UserSettings.EGender Gender
        {
            get { return _settings.User.Gender; }
            set { _settings.User.Gender = value; }
        }

        [SettingString(LabelWidth = "80", Prefix = "Sir", Suffix = "the Great")]
        public string Name
        {
            get { return _settings.User.Name; }
            set { _settings.User.Name = value; }
        }

        [SettingNumber(LabelWidth = "80", MinValue = 0, MaxValue = 160)]
        public int Age
        {
            get { return _settings.User.Age; }
            set { _settings.User.Age = value; }
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
             LabelWidth = "100",
             Icon = "Resources.icon-bulb.png")]
        public GeneralSettings General { get; }

        [SettingSection(
             LabelWidth = "100",
             Icon = "Resources.icon-skull.png")]
        public UserSettings User { get; }

        [SettingSection(
             LabelWidth = "0.3*",
             Expansion = SectionExpansion.Expanded,
             Icon = "Resources.icon-window-system.png")]
        public InterfaceSettings Interface { get; }

        [SettingSection(
             LabelWidth = "Auto",
             Expansion = SectionExpansion.Expanded,
             Icon = "Resources.icon-globe.png")]
        public CulturalSettings Culture { get; }

        public GlobalSettings()
        {
            General = new GeneralSettings();
            User = new UserSettings();
            Interface = new InterfaceSettings();
            Culture = new CulturalSettings();
        }
    }

    [ImplementPropertyChanged]
    public class GeneralSettings
    {
        [SettingBool(Label = "Launch this program with Windows start-up",
             Details = "This option doesn't actually have any effect, don't worry.")]
        public bool StartReboot { get; set; }

        [SettingNumber(Type = NumberSettingType.SliderAndValue,
             MinValue = 100, MaxValue = 250, TickFrequency = 50,
             SuffixLabel = "%")]
        public int ElementsSizes { get; set; } = 100;

        [SettingButton(Label = "Style settings", Type = ButtonType.Link)]
        public string LinkToStyle { get; } = "Interface.Style";
    }

    [ImplementPropertyChanged]
    [SettingGroupDefinition(Type = GroupType.Title, Position = 0, Name = "GeneralInformation")]
    [SettingGroupDefinition(Type = GroupType.Title, Position = 1, Name = "UsefulInformation")]
    public class UserSettings
    {
        public enum EGender
        {
            [SettingField(Details = "A male color is blue.")] Male,
            [SettingField(Details = "A female color is pink.")] Female,
            [SettingField(Details = "Another color is green.")] Other
        }

        [SettingChoice(
             Details = "This will change the background to match your color preferences! No questions.",
             InGroup = "GeneralInformation")]
        public EGender Gender { get; set; } = EGender.Other;

        [SettingString(Prefix = "Sir", Suffix = "the Great",
             InGroup = "GeneralInformation")]
        public string Name { get; set; } = "Bob";

        [SettingNumber(MinValue = 0, MaxValue = 160,
             InGroup = "GeneralInformation")]
        public int Age { get; set; } = 42;

        [SettingNumber(Type = NumberSettingType.SliderAndSpinner, MinValue = 1900, MaxValue = 2020,
             TickFrequency = 10, InGroup = "UsefulInformation", Details = "Just to make sure...")]
        public int YearOfBirth { get; set; } = 1980;

        [SettingDate(InGroup = "UsefulInformation", Details = "Triple check!")]
        public DateTime Birthday { get; set; }

        [SettingString(Prefix = "https://",
             InGroup = "UsefulInformation")]
        public string Website { get; set; } = "duckduckgo.com";
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
            [SettingField(Label = "Aggressive", Details = "Or bold, as one could say.")] Bold,
            [SettingField(Label = "Discrete")] Italic
        }

        [SettingChoice(Position = 1, Type = SettingChoiceType.RadioButtons)]
        public TextStyle Style { get; set; } = TextStyle.Bold;

        // TODO: color picker
        [SettingString(Position = 0)]
        public string Color { get; set; }
    }

    [ImplementPropertyChanged]
    public class StyleSettings
    {
        [SettingGroup(Position = 2, LabelWidth = "70")]
        public BoxStyle ContentStyle { get; }

        [SettingGroup(Position = 1, LabelWidth = "70")]
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

        [SettingText(Height = "*")]
        public string PageContent { get; set; }

        [SettingButton(Label = "Randomize content", Alignment = HorizontalAlignment.Right)]
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

    [ImplementPropertyChanged]
    public class CulturalSettings
    {
        [SettingSection(Icon = "Resources.icon-bubble.png")]
        public LanguageSettings Language { get; }

        [SettingSection(Icon = "Resources.icon-compass.png")]
        public LocalizationSettings Localization { get; }

        public CulturalSettings()
        {
            Language = new LanguageSettings();
            Localization = new LocalizationSettings();
        }
    }

    public class Language
    {
        /// <summary>
        ///     ISO 639-1 code.
        /// </summary>
        public string Code { get; }

        public string Name { get; }

        public Language(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }

    [ImplementPropertyChanged]
    public class LanguageSettings
    {
        [SettingBool]
        public bool UseSystemLanguage { get; set; } = true;

        [SettingChoice(ItemsSource = nameof(Languages), Height = "*",
             Type = SettingChoiceType.ListView,
             ItemsLabelPath = "Name")]
        public string DisplayLanguage { get; set; } = "En";

        public List<Language> Languages { get; } = new List<Language>
        {
            new Language("de", "German"),
            new Language("en", "English"),
            new Language("es", "Spanish"),
            new Language("fr", "French"),
            new Language("it", "Italian"),
            new Language("ja", "Japanese"),
            new Language("ko", "Korean")
        };
    }

    public class Currency
    {
        /// <summary>
        ///     ISO 4217 code.
        /// </summary>
        public string Code { get; }

        public string Symbol { get; }
        public string Name { get; }

        public Currency(string code, string symbol, string name)
        {
            Code = code;
            Symbol = symbol;
            Name = name;
        }
    }

    public enum TemperatureUnit
    {
        [SettingField] Celsius,
        [SettingField] Fahrenheit,
        [SettingField] Kelvin,
        [SettingField] Rankine,
        [SettingField] Delisle,
        [SettingField] Newton,
        [SettingField] Réaumur,
        [SettingField] Rømer
    }


    [ImplementPropertyChanged]
    public class LocalizationSettings
    {
        [SettingString]
        public string TimeFormat { get; set; } = "yyyy-MM-dd";

        [SettingChoice(ItemsSource = nameof(Currencies), ItemsLabelPath = "Name")]
        public Currency Currency { get; set; }

        [SettingChoice]
        public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.Celsius;

        public List<Currency> Currencies { get; } = new List<Currency>
        {
            new Currency("USD", "$", "United States dollar"),
            new Currency("EUR", "€", "Euro"),
            new Currency("JPY", "¥", "Japanese Yen"),
            new Currency("GBP", "£", "Pound sterling")
        };

        public LocalizationSettings()
        {
            Currency = Currencies[1];
        }
    }
}
