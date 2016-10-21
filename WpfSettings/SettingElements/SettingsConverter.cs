using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using WpfSettings.Utils;
using WpfSettings.Utils.Reflection;

namespace WpfSettings.SettingElements
{
    internal static class SettingsConverter
    {
        public static ObservableCollection<SettingSection> GetSections(object settings, ConverterArgs e)
        {
            MemberInfo[] members = settings.GetType().GetMembers();
            var sections = members
                .Where(IsSection)
                .Select(p => GetSection(settings, p, e))
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Member.MetadataToken);
            return new ObservableCollection<SettingSection>(sections);
        }

        public static ObservableCollection<SettingPageElement> GetElements(object settings, ConverterArgs e)
        {
            MemberInfo[] members = settings.GetType().GetMembers();
            var elements = members
                .Select(p => GetElement(settings, p, e))
                .Where(el => el != null)
                .OrderBy(el => el.Position)
                .ThenBy(el => el.Member.MetadataToken);
            return new ObservableCollection<SettingPageElement>(elements);
        }

        private static bool IsSection(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
            return attribute != null;
        }

        private static SettingSection GetSection(object parent, MemberInfo member, ConverterArgs e)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
            object value = member.GetValue(parent);

            e = e.Integrate(attribute);
            SettingSection section = new SettingSection(parent, member)
            {
                IsExpanded = e.Expansion == SectionExpansion.Expanded ||
                             e.Expansion == SectionExpansion.ExpandedRecursive
            };
            e = e.NextArgs(attribute);
            section.SubSections = GetSections(value, e);
            section.Elements = GetElements(value, e);
            if (!string.IsNullOrEmpty(attribute.Label))
                section.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Icon))
            {
                var stream = ResourceUtils.FromParentAssembly(attribute.Icon);
                var image = new Bitmap(stream);
                section.Icon = image.ToBitmapSource();
            }
            section.Position = attribute.Position;
            return section;
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            var attributes = member.GetCustomAttributes(false);
            // Return the first valid attribute
            foreach (object attribute in attributes)
            {
                SettingPageElement element = GetElement(parent, member, (dynamic) attribute, e);
                if (element != null)
                    return element;
            }
            return null;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static SettingPageElement GetElement(object parent, MemberInfo member,
            object attribute, ConverterArgs e)
        {
            return null;
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member,
            SettingGroupAttribute attribute, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (!type.IsClass)
                throw new ArgumentException("SettingGroupAttribute must target a class (not a value type or interface)");
            object value = member.GetValue(parent);
            e = e.Integrate(attribute);
            e = e.NextArgs(attribute);
            var elements = GetElements(value, e);
            SettingGroup element = new SettingGroup(parent, member, elements);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            element.Position = attribute.Position;
            element.AutoSave = e.AutoSave;
            return element;
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member,
            SettingStringAttribute attribute, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingStringAttribute must target a string");
            e = e.Integrate(attribute);
            StringSetting element = new StringSetting(parent, member);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (string) member.GetValue(parent);
            element.LabelWidth = e.LabelWidth;
            element.AutoSave = e.AutoSave;
            return element;
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member,
            SettingTextAttribute attribute, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingTextAttribute must target a string");
            e = e.Integrate(attribute);
            TextSetting element = new TextSetting(parent, member);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            if (attribute.Height > 0)
                element.Height = attribute.Height;
            element.Position = attribute.Position;
            element.Value = (string) member.GetValue(parent);
            element.LabelWidth = e.LabelWidth;
            element.AutoSave = e.AutoSave;
            return element;
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member,
            SettingBoolAttribute attribute, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(bool))
                throw new ArgumentException("SettingBoolAttribute must target a boolean");
            e = e.Integrate(attribute);
            BoolSetting element = new BoolSetting(parent, member);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Value = (bool) member.GetValue(parent);
            element.LabelWidth = e.LabelWidth;
            element.AutoSave = e.AutoSave;
            return element;
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member,
            SettingChoiceAttribute attribute, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (!type.IsEnum)
                throw new ArgumentException("SettingChoiceAttribute must target an enum");
            e = e.Integrate(attribute);
            var choices = new ObservableCollection<string>();
            Array names = Enum.GetNames(type);
            foreach (string name in names)
            {
                string label = GetFieldLabel(type, name);
                if (!string.IsNullOrEmpty(label))
                    choices.Add(label);
            }
            var element = attribute.Type == ChoiceType.DropDown
                ? (ChoiceSetting) new DropDownSetting(parent, member)
                : new RadioButtonsSetting(parent, member);
            element.Choices = choices;
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            string enumValue = GetFieldLabel(type, member.GetValue(parent).ToString());
            element.SelectedValue = enumValue;
            element.LabelWidth = e.LabelWidth;
            element.AutoSave = e.AutoSave;
            return element;
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member,
            SettingButtonAttribute attribute, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(Action))
                throw new ArgumentException("SettingButtonAttribute must target an Action");
            e = e.Integrate(attribute);
            ButtonSetting element = new ButtonSetting(parent, member);
            if (!string.IsNullOrEmpty(attribute.Label))
                element.Label = attribute.Label;
            if (!string.IsNullOrEmpty(attribute.Details))
                element.Details = attribute.Details;
            element.Position = attribute.Position;
            element.Action = (Action) member.GetValue(parent);
            element.LabelWidth = e.LabelWidth;
            element.AutoSave = e.AutoSave;
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
