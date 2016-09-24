using System.Collections.ObjectModel;

namespace WpfSettings.Config
{
    internal class ConfigManager
    {
        public object ExternalConfig { get; private set; }
        // TODO: simple array?
        public ObservableCollection<ConfigSection> InternalConfig { get; private set; }

        public ConfigManager(object config)
        {
            ExternalConfig = config;
        }

        public ObservableCollection<ConfigSection> ConvertConfig()
        {
            //PropertyInfo[] properties = ExternalConfig.GetType()
            //    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var items = new ObservableCollection<ConfigSection>();
            var section = new ConfigSection("general", "General");

            //ConfigElements.Add(new TitleConfig("test0", "My title"));
            section.Add(new StringConfig("test1", "My string"));
            section.Add(new TextConfig("test1", "My multiple line answer"));
            section.Add(new BoolConfig("test2", "My checkbox"));
            section.Add(new BoolConfig("test2", "My second checkbox"));
            //config = new TitleConfig("test0", "A title with details");
            //config.Details = "This part deals with other kinds of issues";
            //ConfigElements.Add(config);
            section.Add(new BoolConfig("test2", "A checkbox on\ntwo lines"));
            section.Add(new BoolConfig("test2", "Checkbox with details")
                {
                    Details =
                        "You can either activate or deactivate this checkbox." +
                        "It's made for it! By default it's deactivated so" +
                        "don't worry and be happy. We will take care of everything."
                }
            );
            section.Add(new ChoiceConfig("test2", "Select the value",
                new ObservableCollection<string> {"First Value", "Second Value"}));

            //section.Image = "icon-search.png";
            items.Add(section);
            section = new ConfigSection("env", "Environment");
            section.SubSections.Add(new ConfigSection("test1", "General"));
            section.SubSections.Add(new ConfigSection("test2", "Documents"));
            items.Add(section);

            InternalConfig = items;

            return items;
        }

        public void SaveConfig()
        {
            // TODO: Save properties
        }
    }
}
