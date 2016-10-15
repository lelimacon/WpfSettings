using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using WpfSettings.Utils;

namespace WpfSettings.Config
{
    internal static class SettingsConverter
    {
        public static ObservableCollection<ConfigSection> GetSections(object settings)
        {
            MemberInfo[] members = settings.GetType().GetMembers();
            var sections = members
                .Where(IsSection)
                .Select(p => GetSection(settings, p))
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Member.MetadataToken);
            return new ObservableCollection<ConfigSection>(sections);
        }

        public static ObservableCollection<ConfigPageElement> GetElements(object settings)
        {
            MemberInfo[] members = settings.GetType().GetMembers();
            var elements = members
                .Select(p => GetElement(settings, p))
                .Where(e => e != null)
                .OrderBy(e => e.Position)
                .ThenBy(e => e.Member.MetadataToken);
            return new ObservableCollection<ConfigPageElement>(elements);
        }

        private static bool IsSection(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
            return attribute != null;
        }

        private static ConfigSection GetSection(object parent, MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
            object value = member.GetValue(parent);
            var sections = GetSections(value);
            var elements = GetElements(value);
            ConfigSection section = new ConfigSection(parent, member)
            {
                SubSections = sections,
                Elements = elements
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

        private static ConfigPageElement GetElement(object parent, MemberInfo member)
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

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static ConfigPageElement GetElement(object parent, MemberInfo member, object att)
        {
            return null;
        }

        private static ConfigPageElement GetElement(object parent, MemberInfo member, SettingGroupAttribute attribute)
        {
            Type type = member.GetValueType();
            object value = member.GetValue(parent);
            var elements = GetElements(value);
            if (!type.IsClass)
                throw new ArgumentException("SettingGroupAttribute must target a class (not a value type or interface)");
            ConfigGroup element = new ConfigGroup(parent, member, elements);
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
