namespace WpfSettings.Example
{
    public class MyConfig
    {
        public GeneralSettings General { get; set; }
    }

    [SettingSection("General Settings")]
    public class GeneralSettings
    {
        public enum MyEnum
        {
            [SettingField("Choix 1")]
            FirstChoice,

            [SettingField("Choix 2")]
            FirstChoice2,
        }

        [SettingField("Click me")]
        public bool Checkbox { get; set; }

        [SettingField("Your name", DefaultValue = "Alibaba")]
        public string Login { get; set; }

        [SettingField("Choose one")]
        public MyEnum MyChoice { get; set; }
    }
}
