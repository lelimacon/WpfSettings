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
            var sections = properties
                .Where(IsSection)
                .Select(p => GetSettingSection(ExternalConfig, p))
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Property.MetadataToken);
            InternalConfig = new ObservableCollection<ConfigSection>(sections);
            return InternalConfig;
        }

        private static bool IsSection(PropertyInfo p)
        {
            var attribute = p.GetCustomAttribute<SettingSectionAttribute>(false);
            return attribute != null;
        }

        private ConfigSection GetSettingSection(object parent, PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<SettingSectionAttribute>(false);
            PropertyInfo[] properties = prop.PropertyType.GetProperties();
            object value = prop.GetValue(parent);
            var sections = properties
                .Where(IsSection)
                .Select(p => GetSettingSection(value, p))
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Property.MetadataToken);
            var elements = properties
                .Select(p => GetSettingElement(value, p))
                .Where(e => e != null)
                .OrderBy(e => e.Position)
                .ThenBy(e => e.Property.MetadataToken);
            ConfigSection section = new ConfigSection(parent, prop)
            {
                SubSections = new ObservableCollection<ConfigSection>(sections),
                Elements = new ObservableCollection<ConfigPageElement>(elements)
            };
            if (!string.IsNullOrEmpty(attribute.Label))
                section.Label = attribute.Label;
            section.Position = attribute.Position;
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
            element.Position = attribute.Position;
            element.Value = (string)prop.GetValue(parent);
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
            element.Position = attribute.Position;
            element.Value = (string)prop.GetValue(parent);
            return element;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, SettingBoolAttribute attribute)
        {
            Type type = prop.PropertyType;
            if (type != typeof(bool))
                throw new ArgumentException("SettingBoolAttribute must target a boolean");
            BoolConfig element = new BoolConfig(parent, prop);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            element.Position = attribute.Position;
            element.Value = (bool)prop.GetValue(parent);
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
                string label = GetFieldLabel(type, name);
                if (!string.IsNullOrEmpty(label))
                    choices.Add(label);
            }
            ChoiceConfig element;
            if (attribute.Type == ChoiceType.DropDown)
                element = new DropDownConfig(parent, prop);
            else
                element = new RadioButtonsConfig(parent, prop);
            element.Choices = choices;
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            element.Position = attribute.Position;
            string enumValue = GetFieldLabel(type, prop.GetValue(parent).ToString());
            element.SelectedValue = enumValue;
            return element;
        }

        private static string GetFieldLabel(Type enumType, string fieldName)
        {
            var memberInfos = enumType.GetMember(fieldName);
            var attr = memberInfos[0].GetCustomAttribute<SettingFieldAttribute>(false);
            return attr?.Label;
        }
    }
}
