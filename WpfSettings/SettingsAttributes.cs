using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
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

        /// <summary>
        ///     Gets or sets the order to display the settings.
        ///     Defaults to the declaration position.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        ///     Gets or sets the label on the left-side of the actual setting.
        ///     This property is inferred if not specified.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     Gets or sets the labels width. Must be greater or equal than 0.
        ///     This is a recursive parameter.
        ///     Defaults to 140.
        /// </summary>
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
        /// <summary>
        ///     Gets or sets the icon path.
        ///     The image will be retrieved from the calling assembly.
        ///     Make sure the target image Build Action is Embedded Resource.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Gets or sets the expansion to apply to the section in tree.
        ///     This is a recursive parameter.
        ///     Defaults to unset (closed).
        /// </summary>
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
        /// <summary>
        ///     Gets or sets detailed information to display under the setting.
        /// </summary>
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
        /// <summary>
        ///     Gets or sets a secondary label to display on the right.
        /// </summary>
        public string SuffixLabel { get; set; }

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingStringAttribute must target a string");
            e = e.Integrate(this);
            StringSetting element = new StringSetting(parent, member);
            Fill(element, member, e);
            element.Value = (string) member.GetValue(parent);
            element.SuffixLabel = SuffixLabel;
            return element;
        }
    }

    public class SettingNumberAttribute : SettingPageAttribute
    {
        /// <summary>
        ///     Gets or sets the minimum value.
        ///     Defaults to min int.
        /// </summary>
        public string MinValue { get; set; }

        /// <summary>
        ///     Gets or sets the minimum value.
        ///     Defaults to max int.
        /// </summary>
        public string MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the ticks frequency on the slider.
        ///     Defaults to 0 (two ticks, under min and max values).
        /// </summary>
        public string TickFrequency { get; set; }

        /// <summary>
        ///     Gets or sets the step for the spinner.
        ///     Defaults to 1.
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        ///     Gets or sets the type of display of the number.
        ///     Defaults to the spinner.
        /// </summary>
        public NumberSettingType Type { get; set; }

        /// <summary>
        ///     Gets or sets a secondary label to display on the right.
        /// </summary>
        public string SuffixLabel { get; set; }

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(int))
                throw new ArgumentException("SettingNumberAttribute must target an integer");
            e = e.Integrate(this);
            NumberSetting element = new NumberSetting(parent, member);
            Fill(element, member, e);
            element.Value = (int) member.GetValue(parent);
            element.SuffixLabel = SuffixLabel;
            if (!string.IsNullOrEmpty(MinValue))
                element.MinValue = GetInt(MinValue);
            if (!string.IsNullOrEmpty(MaxValue))
                element.MaxValue = GetInt(MaxValue);
            if (!string.IsNullOrEmpty(TickFrequency))
                element.TickFrequency = GetInt(TickFrequency);
            element.Step = Step;
            element.Type = Type;
            return element;
        }

        private int GetInt(string input)
        {
            int value;
            bool parsed = int.TryParse(input, out value);
            if (!parsed)
                throw new ArgumentException("Value must be an integer");
            return value;
        }
    }

    public class SettingTextAttribute : SettingPageAttribute
    {
        private int _height;

        /// <summary>
        ///     Gets or sets the height of the TextBox.
        ///     Defaults to 60.
        /// </summary>
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

    public class SettingDateAttribute : SettingPageAttribute
    {
        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(DateTime))
                throw new ArgumentException("SettingStringAttribute must target a DateTime");
            e = e.Integrate(this);
            DateSetting element = new DateSetting(parent, member);
            Fill(element, member, e);
            element.Value = (DateTime) member.GetValue(parent);
            return element;
        }
    }

    public enum SettingChoiceType
    {
        DropDown,
        RadioButtons
    }

    public class SettingChoiceAttribute : SettingPageAttribute
    {
        /// <summary>
        ///     Gets or sets the type of display of the dropdown.
        ///     Defaults to the spinner.
        /// </summary>
        public SettingChoiceType Type { get; set; } = SettingChoiceType.DropDown;

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (!type.IsEnum)
                throw new ArgumentException("SettingChoiceAttribute must target an enum");
            e = e.Integrate(this);
            var choices = new ObservableCollection<SettingField>();
            Array names = Enum.GetNames(type);
            foreach (string name in names)
            {
                MemberInfo memberInfo = type.GetMember(name)[0];
                var attribute = memberInfo.GetCustomAttribute<SettingFieldAttribute>(false);
                if (attribute == null)
                    continue;
                SettingField field = attribute.GetElement(parent, memberInfo);
                choices.Add(field);
            }
            var element = Type == SettingChoiceType.DropDown
                ? (ChoiceSetting) new DropDownSetting(parent, member)
                : new RadioButtonsSetting(parent, member);
            Fill(element, member, e);
            element.Choices = choices;
            var value = member.GetValue(parent);
            element.SelectedValue = choices.FirstOrDefault(a => a.Value.Equals(value));
            return element;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingFieldAttribute : SettingAttribute
    {
        /// <summary>
        ///     Gets or sets detailed information to display under the setting.
        /// </summary>
        public string Details { get; set; }

        internal SettingField GetElement(object parent, MemberInfo member)
        {
            string label = Label ?? SettingsConverter.InferLabel(member.Name);
            SettingField field = new SettingField(member.GetValue(parent),
                member.Name, label, Details);
            return field;
        }
    }

    public class SettingButtonAttribute : SettingPageAttribute
    {
        /// <summary>
        ///     Gets or sets the horizontal alignment of the button.
        ///     Defaults to Left.
        /// </summary>
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
