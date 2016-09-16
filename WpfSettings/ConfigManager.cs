using System.Collections.ObjectModel;
using System.Reflection;

namespace WpfSettingsControl
{
    public class ConfigManager
    {
        public object ExternalConfig { get; private set; }
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
