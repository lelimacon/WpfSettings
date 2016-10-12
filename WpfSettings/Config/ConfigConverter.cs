using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using WpfSettings.Utils;

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

        private static ConfigSection GetSettingSection(object parent, PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<SettingSectionAttribute>(false);
            PropertyInfo[] properties = prop.PropertyType.GetProperties();
            object value = prop.GetValue(parent);
            var sections = GetSections(properties, value);
            var elements = GetElements(properties, value);
            ConfigSection section = new ConfigSection(parent, prop)
            {
                SubSections = new ObservableCollection<ConfigSection>(sections),
                Elements = new ObservableCollection<ConfigPageElement>(elements)
            };
            if (!string.IsNullOrEmpty(attribute.Label))
                section.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Image))
            {
                var stream = ResourceUtils.FromParentAssembly(attribute.Image);
                var image = new Bitmap(stream);
                section.Image = image.ToBitmapSource();
            }
            section.Position = attribute.Position;
            return section;
        }

        private static ConfigPageElement GetSettingElement(object parent, PropertyInfo prop)
        {
            var attributes = prop.GetCustomAttributes(false);
            foreach (object attribute in attributes)
            {
                ConfigPageElement element = GetElement(parent, prop, (dynamic) attribute);
                if (element != null)
                    return element;
            }
            return null;
        }

        private static IOrderedEnumerable<ConfigSection> GetSections(PropertyInfo[] properties, object value)
        {
            var sections = properties
                .Where(IsSection)
                .Select(p => GetSettingSection(value, p))
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Property.MetadataToken);
            return sections;
        }

        private static IOrderedEnumerable<ConfigPageElement> GetElements(PropertyInfo[] properties, object value)
        {
            var elements = properties
                .Select(p => GetSettingElement(value, p))
                .Where(e => e != null)
                .OrderBy(e => e.Position)
                .ThenBy(e => e.Property.MetadataToken);
            return elements;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, object att)
        {
            return null;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, SettingGroupAttribute attribute)
        {
            PropertyInfo[] properties = prop.PropertyType.GetProperties();
            object value = prop.GetValue(parent);
            var elements = GetElements(properties, value);
            Type type = prop.PropertyType;
            if (!type.IsClass)
                throw new ArgumentException("SettingGroupAttribute must target a class (not a value type or interface)");
            ConfigGroup element = new ConfigGroup(parent, prop)
            {
                Elements = new ObservableCollection<ConfigPageElement>(elements)
            };
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            element.Position = attribute.Position;
            return element;
        }

        private static ConfigPageElement GetElement(object parent, PropertyInfo prop, SettingStringAttribute attribute)
        {
            Type type = prop.PropertyType;
            if (type != typeof(string))
                throw new ArgumentException("SettingStringAttribute must target a string");
            StringConfig element = new StringConfig(parent, prop);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (string) prop.GetValue(parent);
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
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (string) prop.GetValue(parent);
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
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (bool) prop.GetValue(parent);
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
            var element = attribute.Type == ChoiceType.DropDown
                ? (ChoiceConfig) new DropDownConfig(parent, prop)
                : new RadioButtonsConfig(parent, prop);
            element.Choices = choices;
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
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
