using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace WpfSettings.Config
{
    internal class ConfigConverter
    {
        public object ExternalConfig { get; }
        public ObservableCollection<ConfigSection> InternalConfig { get; private set; }

        public ConfigConverter(object config)
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

        private ConfigSection GetSettingSection(object parent, PropertyInfo prop)
        {
            var attribute = prop.PropertyType.GetCustomAttribute<SettingSectionAttribute>(true);
            PropertyInfo[] properties = prop.PropertyType.GetProperties();
            object value = prop.GetValue(parent);
            var sections = properties
                .Where(IsSection)
                .Select(p => GetSettingSection(value, p));
            var elements = properties
                .Select(p => GetSettingElement(value, p))
                .Where(e => e != null);
            ConfigSection section = new ConfigSection(parent, prop)
            {
                SubSections = new ObservableCollection<ConfigSection>(sections),
                Elements = new ObservableCollection<ConfigPageElement>(elements)
            };
            if (!string.IsNullOrEmpty(attribute.Label))
                section.Label = attribute.Label;
            return section;
        }

        private ConfigPageElement GetSettingElement(object parent, PropertyInfo prop)
        {
            // TODO: Retrieve all attributes
            Type type = prop.PropertyType;
            var attributes = prop.GetCustomAttributes(false);
            foreach (object attribute in attributes)
            {
                ConfigPageElement element = GetElement(parent, prop, (dynamic) attribute);
                if (element != null)
                    return element;
            }
            return null;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, object att)
        {
            return null;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, SettingStringAttribute attribute)
        {
            Type type = prop.PropertyType;
            if (type != typeof(string))
                throw new ArgumentException("SettingStringAttribute must target a string");
            StringConfig element = new StringConfig(parent, prop);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            return element;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, SettingTextAttribute attribute)
        {
            Type type = prop.PropertyType;
            if (type != typeof(string))
                throw new ArgumentException("SettingTextAttribute must target a string");
            TextConfig element = new TextConfig(parent, prop);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            return element;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, SettingBoolAttribute attribute)
        {
            Type type = prop.PropertyType;
            if (type != typeof(bool))
                throw new ArgumentException("SettingTextAttribute must target a boolean");
            BoolConfig element = new BoolConfig(parent, prop);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            return element;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, SettingChoiceAttribute attribute)
        {
            Type type = prop.PropertyType;
            if (!type.IsEnum)
                throw new ArgumentException("SettingChoiceAttribute must target an enum");
            var choices = new ObservableCollection<string>();
            Array names = Enum.GetNames(type);
            foreach (string name in names)
            {
                var memberInfos = type.GetMember(name);
                var attr = memberInfos[0].GetCustomAttribute<SettingFieldAttribute>(false);
                if (attr != null)
                    choices.Add(attr.Label);
            }
            ChoiceConfig element;
            if (attribute.Type == ChoiceType.DropDown)
                element = new DropDownConfig(parent, prop);
            else
                element = new RadioButtonsConfig(parent, prop);
            element.Choices = choices;
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            return element;
        }
    }
}
