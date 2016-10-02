using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace WpfSettings.Config
{
    internal class ConfigManager
    {
        public object ExternalConfig { get; }
        // TODO: simple array?
        public ObservableCollection<ConfigSection> InternalConfig { get; private set; }

        public ConfigManager(object config)
        {
            ExternalConfig = config;
        }

        public ObservableCollection<ConfigSection> ConvertConfig()
        {
            PropertyInfo[] properties = ExternalConfig.GetType().GetProperties();
            var infos = properties.Where(IsSection);
            var sections = infos
                .Select(p => GetSettingSection(ExternalConfig, p));
            InternalConfig = new ObservableCollection<ConfigSection>(sections);
            return InternalConfig;
        }

        private static bool IsSection(PropertyInfo p)
        {
            var attribute = p.PropertyType.GetCustomAttribute<SettingSectionAttribute>(true);
            return attribute != null;
        }

        private static bool IsField(PropertyInfo p)
        {
            object[] attributes = p.GetCustomAttributes(true);
            return attributes.Any(a => a is SettingFieldAttribute);
        }

        private ConfigSection GetSettingSection(object parent, PropertyInfo prop)
        {
            var attribute = prop.PropertyType.GetCustomAttribute<SettingSectionAttribute>(true);
            PropertyInfo[] properties = prop.PropertyType.GetProperties();
            var sections = properties
                .Where(IsSection)
                .Select(p => GetSettingSection(ExternalConfig, p));
            var items = properties
                .Where(IsField)
                .Select(p => GetSettingElement(ExternalConfig, p));
            ConfigSection section = new ConfigSection(parent, prop)
            {
                SubSections = new ObservableCollection<ConfigSection>(sections),
                Elements = new ObservableCollection<ConfigPageElement>(items)
            };
            if (!string.IsNullOrEmpty(attribute.Label))
                section.Label = attribute.Label;
            return section;
        }

        private ConfigPageElement GetSettingElement(object parent, PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<SettingFieldAttribute>(false);
            Type type = prop.PropertyType;
            ConfigPageElement element;
            if (type == typeof(string))
                element = new StringConfig(parent, prop);
            else if (type == typeof(bool))
                element = new BoolConfig(parent, prop);
            else if (type.IsEnum)
            {
                element = GetChoiceConfig(parent, prop);
            }
            else
                throw new ArgumentException($"Type of \"{prop.Name}\" is not recognized");
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            return element;
        }

        private static ChoiceConfig GetChoiceConfig(object parent, PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            var choices = new ObservableCollection<string>();
            Array names = Enum.GetNames(type);
            foreach (string name in names)
            {
                var memberInfos = type.GetMember(name);
                var attr = memberInfos[0].GetCustomAttribute<SettingFieldAttribute>(false);
                if (attr != null)
                    choices.Add(attr.Label);
            }
            return new ChoiceConfig(parent, prop)
            {
                Choices = choices,
            };
        }

        public void SaveConfig()
        {
            // TODO: Save properties
        }
    }
}
