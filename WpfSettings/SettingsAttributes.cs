using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows;
using WpfSettings.SettingElements;
using WpfSettings.Utils;
using WpfSettings.Utils.Reflection;

namespace WpfSettings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class SettingAttribute : Attribute
    {
        private int _labelWidth;

        public int Position { get; set; }
        public string Label { get; set; }

        public int LabelWidth
        {
            get { return _labelWidth; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("LabelWidth must be > 0");
                _labelWidth = value;
            }
        }
    }

    public enum SectionExpansion
    {
        Unset,
        Closed,
        Expanded,
        ExpandedRecursive,
        ChildrenExpanded,
        ChildrenExpandedRecursive
    }

    public class SettingSectionAttribute : SettingAttribute
    {
        public string Icon { get; set; }
        public SectionExpansion Expansion { get; set; } = SectionExpansion.Unset;

        internal SettingSection GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            object value = member.GetValue(parent);

            e = e.Integrate(this);
            SettingSection section = new SettingSection(parent, member)
            {
                IsExpanded = e.Expansion == SectionExpansion.Expanded ||
                             e.Expansion == SectionExpansion.ExpandedRecursive
            };
            e = e.NextArgs(this);
            section.SubSections = SettingsConverter.GetSections(value, e);
            section.Elements = SettingsConverter.GetElements(value, e);
            section.Label = Label ?? SettingsConverter.InferLabel(member.Name);
            if (!string.IsNullOrEmpty(Icon))
            {
                var stream = ResourceUtils.FromParentAssembly(Icon);
                var image = new Bitmap(stream);
                section.Icon = image.ToBitmapSource();
            }
            section.Position = Position;
            return section;
        }
    }

    public abstract class SettingPageAttribute : SettingAttribute
    {
        public string Details { get; set; }

        internal abstract SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e);

        internal void Fill(SettingPageElement element, MemberInfo member, ConverterArgs e)
        {
            element.Label = Label ?? SettingsConverter.InferLabel(member.Name);
            element.LabelWidth = e.LabelWidth;
            if (Details != null)
                element.Details = Details;
            element.Position = Position;
            element.AutoSave = e.AutoSave;
        }
    }

    public class SettingGroupAttribute : SettingPageAttribute
    {
        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (!type.IsClass)
                throw new ArgumentException("SettingGroupAttribute must target a class (not a value type or interface)");
            object value = member.GetValue(parent);
            e = e.Integrate(this);
            e = e.NextArgs(this);
            var elements = SettingsConverter.GetElements(value, e);
            SettingGroup element = new SettingGroup(parent, member, elements);
            Fill(element, member, e);
            return element;
        }
    }

    public class SettingStringAttribute : SettingPageAttribute
    {
        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingStringAttribute must target a string");
            e = e.Integrate(this);
            StringSetting element = new StringSetting(parent, member);
            Fill(element, member, e);
            element.Value = (string) member.GetValue(parent);
            return element;
        }
    }

    public class SettingNumberAttribute : SettingPageAttribute
    {
        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(int))
                throw new ArgumentException("SettingNumberAttribute must target an integer");
            e = e.Integrate(this);
            NumberSetting element = new NumberSetting(parent, member);
            Fill(element, member, e);
            element.Value = (int) member.GetValue(parent);
            return element;
        }
    }

    public class SettingTextAttribute : SettingPageAttribute
    {
        private int _height;

        public int Height
        {
            get { return _height; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Height must be > 0");
                _height = value;
            }
        }

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingTextAttribute must target a string");
            e = e.Integrate(this);
            TextSetting element = new TextSetting(parent, member);
            Fill(element, member, e);
            element.Value = (string) member.GetValue(parent);
            if (Height > 0)
                element.Height = Height;
            return element;
        }
    }

    public class SettingBoolAttribute : SettingPageAttribute
    {
        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(bool))
                throw new ArgumentException("SettingBoolAttribute must target a boolean");
            e = e.Integrate(this);
            BoolSetting element = new BoolSetting(parent, member);
            Fill(element, member, e);
            element.Value = (bool) member.GetValue(parent);
            return element;
        }
    }

    public enum ChoiceType
    {
        DropDown,
        RadioButtons
    }

    public class SettingChoiceAttribute : SettingPageAttribute
    {
        public string GroupName { get; set; }
        public ChoiceType Type { get; set; } = ChoiceType.DropDown;

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (!type.IsEnum)
                throw new ArgumentException("SettingChoiceAttribute must target an enum");
            e = e.Integrate(this);
            var choices = new ObservableCollection<string>();
            Array names = Enum.GetNames(type);
            foreach (string name in names)
            {
                string label = GetFieldLabel(type, name);
                if (!string.IsNullOrEmpty(label))
                    choices.Add(label);
            }
            var element = Type == ChoiceType.DropDown
                ? (ChoiceSetting) new DropDownSetting(parent, member)
                : new RadioButtonsSetting(parent, member);
            Fill(element, member, e);
            element.Choices = choices;
            string enumValue = GetFieldLabel(type, member.GetValue(parent).ToString());
            element.SelectedValue = enumValue;
            return element;
        }

        private static string GetFieldLabel(Type enumType, string fieldName)
        {
            var memberInfos = enumType.GetMember(fieldName);
            var attr = memberInfos[0].GetCustomAttribute<SettingFieldAttribute>(false);
            return attr?.Label ?? SettingsConverter.InferLabel(memberInfos[0].Name);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingFieldAttribute : SettingAttribute
    {
    }

    public class SettingButtonAttribute : SettingPageAttribute
    {
        public HorizontalAlignment Alignment { get; set; }

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(Action))
                throw new ArgumentException("SettingButtonAttribute must target an Action");
            e = e.Integrate(this);
            ButtonSetting element = new ButtonSetting(parent, member);
            Fill(element, member, e);
            element.Action = (Action) member.GetValue(parent);
            element.Alignment = Alignment;
            return element;
        }
    }
}
