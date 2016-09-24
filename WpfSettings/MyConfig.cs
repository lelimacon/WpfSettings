namespace WpfSettings
{
    public class MyConfig
    {
        public class GeneralSettings
        {
            public enum MyEnum
            {
                [SettingsLabel("Choix 1")]
                FirstChoice,

                [SettingsLabel("Choix 2")]
                FirstChoice2,
            }

            [SettingsLabel("Click me")]
            public bool Checkbox { get; set; }

            [SettingsLabel("Your name")]
            public string Login { get; set; }

            [SettingsLabel("Choose one")]
            public MyEnum MyChoice { get; set; }
        }

        public GeneralSettings General { get; set; }
    }
}
