﻿using PropertyChanged;
using System;
using System.Windows;
using WpfSettings.SettingElements;

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

        [SettingNumber(LabelWidth = 80, MinValue = "0", MaxValue = "160")]
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

        [SettingSection(
             LabelWidth = 100,
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
             MinValue = "100", MaxValue = "250", TickFrequency = "50",
             SuffixLabel = "%")]
        public int ElementsSizes { get; set; } = 100;
    }

    [ImplementPropertyChanged]
    public class UserSettings
    {
        public enum EGender
        {
            [SettingField(Details = "A male color is blue.")] Male,
            [SettingField(Details = "A female color is pink.")] Female,
            [SettingField(Details = "Another color is green.")] Other
        }

        [SettingChoice(
             Details = "This will change the background to match your color preferences! No questions.")]
        public EGender Gender { get; set; } = EGender.Other;

        [SettingString]
        public string Name { get; set; } = "Bob";

        [SettingNumber(MinValue = "0", MaxValue = "160")]
        public int Age { get; set; } = 42;

        [SettingDate]
        public DateTime Birthday { get; set; }
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

    public enum Language
    {
        En,
        Fr,
        Es
    }

    [ImplementPropertyChanged]
    public class LanguageSettings
    {
        [SettingBool]
        public bool UseSystemLanguage { get; set; } = true;

        [SettingChoice]
        public Language DisplayLanguage { get; set; } = Language.En;
    }

    public enum Currency
    {
        Euro,
        Dollar
    }

    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit,
        Kelvin,
        Rankine,
        Delisle,
        Newton,
        Réaumur,
        Rømer
    }


    [ImplementPropertyChanged]
    public class LocalizationSettings
    {
        [SettingString]
        public string TimeFormat { get; set; } = "yyyy-MM-dd";

        [SettingChoice]
        public Currency Currency { get; set; } = Currency.Euro;

        [SettingChoice]
        public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.Celsius;
    }
}
