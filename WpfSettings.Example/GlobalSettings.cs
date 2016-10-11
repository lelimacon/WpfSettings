﻿using PropertyChanged;

namespace WpfSettings.Example
{
    public class GlobalSettings
    {
        [SettingSection("General")]
        public GeneralSettings General { get; }

        [SettingSection("Profile")]
        public UserSettings User { get; }

        [SettingSection("Interface")]
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
        [SettingGroup(1, "Title Style")]
        public BoxStyle TitleStyle { get; }

        [SettingGroup(2, "Content Style")]
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
