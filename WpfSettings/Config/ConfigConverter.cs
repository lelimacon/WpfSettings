using System;
using System.Collections.Generic;
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
            MemberInfo[] members = ExternalConfig.GetType().GetMembers();
            var sections = GetSections(members, ExternalConfig);
            InternalConfig = new ObservableCollection<ConfigSection>(sections);
            return InternalConfig;
        }

        private static bool IsSection(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
            return attribute != null;
        }

        private static ConfigSection GetSettingSection(object parent, MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
            Type type = member.GetValueType();
            MemberInfo[] members = type.GetMembers();
            object value = member.GetValue(parent);
            var sections = GetSections(members, value);
            var elements = GetElements(members, value);
            ConfigSection section = new ConfigSection(parent, member)
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

        private static ConfigPageElement GetSettingElement(object parent, MemberInfo member)
        {
            var attributes = member.GetCustomAttributes(false);
            // Return the first valid attribute
            foreach (object attribute in attributes)
            {
                ConfigPageElement element = GetElement(parent, member, (dynamic) attribute);
                if (element != null)
                    return element;
            }
            return null;
        }

        private static IOrderedEnumerable<ConfigSection> GetSections(IEnumerable<MemberInfo> members, object parent)
        {
            var sections = members
                .Where(IsSection)
                .Select(p => GetSettingSection(parent, p))
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Member.MetadataToken);
            return sections;
        }

        private static IOrderedEnumerable<ConfigPageElement> GetElements(IEnumerable<MemberInfo> members, object parent)
        {
            var elements = members
                .Select(p => GetSettingElement(parent, p))
                .Where(e => e != null)
                .OrderBy(e => e.Position)
                .ThenBy(e => e.Member.MetadataToken);
            return elements;
        }

        private static ConfigPageElement GetElement(object parent, MemberInfo member, object att)
        {
            return null;
        }

        private static ConfigPageElement GetElement(object parent, MemberInfo member, SettingGroupAttribute attribute)
        {
            Type type = member.GetValueType();
            MemberInfo[] properties = type.GetMembers();
            object value = member.GetValue(parent);
            var elements = GetElements(properties, value);
            if (!type.IsClass)
                throw new ArgumentException("SettingGroupAttribute must target a class (not a value type or interface)");
            ConfigGroup element = new ConfigGroup(parent, member)
            {
                Elements = new ObservableCollection<ConfigPageElement>(elements)
            };
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            element.Position = attribute.Position;
            return element;
        }

        private static ConfigPageElement GetElement(object parent, MemberInfo member, SettingStringAttribute attribute)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingStringAttribute must target a string");
            StringConfig element = new StringConfig(parent, member);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (string) member.GetValue(parent);
            return element;
        }

        private static ConfigPageElement GetElement(object parent, MemberInfo member, SettingTextAttribute attribute)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingTextAttribute must target a string");
            TextConfig element = new TextConfig(parent, member);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (string) member.GetValue(parent);
            return element;
        }

        private static ConfigPageElement GetElement(object parent, MemberInfo member, SettingBoolAttribute attribute)
        {
            Type type = member.GetValueType();
            if (type != typeof(bool))
                throw new ArgumentException("SettingBoolAttribute must target a boolean");
            BoolConfig element = new BoolConfig(parent, member);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (bool) member.GetValue(parent);
            return element;
        }

        private static ConfigPageElement GetElement(object parent, MemberInfo member, SettingChoiceAttribute attribute)
        {
            Type type = member.GetValueType();
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
                ? (ChoiceConfig) new DropDownConfig(parent, member)
                : new RadioButtonsConfig(parent, member);
            element.Choices = choices;
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            string enumValue = GetFieldLabel(type, member.GetValue(parent).ToString());
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
