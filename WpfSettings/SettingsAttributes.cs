﻿using System;
using System.Collections;
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
            e = new ConverterArgs(e, this);
            object value = member.GetValue(parent);
            SettingSection section = new SettingSection(parent, member)
            {
                IsExpanded = e.Expansion == SectionExpansion.Expanded ||
                             e.Expansion == SectionExpansion.ExpandedRecursive
            };
            e = e.ChildrenArgs(this);
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
        ///     Gets or sets the height of the setting row.
        ///     Supports 'Auto', star or absolute values.
        ///     Defaults to Auto.
        /// </summary>
        protected string Height { get; set; }

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
            e = new ConverterArgs(e, this);
            object value = member.GetValue(parent);
            e = e.ChildrenArgs(this);
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
            e = new ConverterArgs(e, this);
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
            e = new ConverterArgs(e, this);
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
        /// <summary>
        ///     Gets or sets the height of the setting row.
        ///     Supports 'Auto', star or absolute values.
        ///     Defaults to 60.
        /// </summary>
        public new string Height
        {
            get { return base.Height; }
            set { base.Height = value; }
        }

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(string))
                throw new ArgumentException("SettingTextAttribute must target a string");
            e = new ConverterArgs(e, this);
            TextSetting element = new TextSetting(parent, member);
            Fill(element, member, e);
            element.Value = (string) member.GetValue(parent);
            element.Height = Height ?? "60";
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
            e = new ConverterArgs(e, this);
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
            e = new ConverterArgs(e, this);
            DateSetting element = new DateSetting(parent, member);
            Fill(element, member, e);
            element.Value = (DateTime) member.GetValue(parent);
            return element;
        }
    }

    public enum SettingChoiceType
    {
        DropDown,
        RadioButtons,
        ListView
    }

    public class SettingChoiceAttribute : SettingPageAttribute
    {
        /// <summary>
        ///     Gets or sets the enumerable name that holds the choices.
        ///     Ignored if type is enum.
        /// </summary>
        public string ItemsSource { get; set; }

        /// <summary>
        ///     Gets or sets the path of the label for the choices.
        ///     Ignored if target type is enum or if items type is primitive string type.
        /// </summary>
        public string ItemsLabelPath { get; set; }

        /// <summary>
        ///     Gets or sets the type of display of the dropdown.
        ///     Defaults to the DropDown.
        /// </summary>
        public SettingChoiceType Type { get; set; } = SettingChoiceType.DropDown;

        /// <summary>
        ///     Gets or sets the height of the listview if choice type is listview.
        ///     Ignored if choice type is not ListView.
        ///     Supports 'Auto', star or absolute values.
        ///     Defaults to 160.
        /// </summary>
        public new string Height
        {
            get { return base.Height; }
            set { base.Height = value; }
        }

        internal override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (!type.IsEnum && ItemsSource == null)
                throw new ArgumentException("SettingChoiceAttribute must target an enum or declare an item source");
            e = new ConverterArgs(e, this);
            var choices = type.IsEnum ? GetEnumChoices(parent, type) : GetListChoices(parent);
            ChoiceSetting element = GetSetting(parent, member);
            Fill(element, member, e);
            element.Height = Type == SettingChoiceType.ListView ? (Height ?? "160") : "Auto";
            element.Choices = choices;
            var value = member.GetValue(parent);
            element.SelectedValue = choices.FirstOrDefault(a => a.Value.Equals(value));
            return element;
        }

        private ChoiceSetting GetSetting(object parent, MemberInfo member)
        {
            switch (Type)
            {
                case SettingChoiceType.DropDown:
                    return new DropDownSetting(parent, member);
                case SettingChoiceType.RadioButtons:
                    return new RadioButtonsSetting(parent, member);
                case SettingChoiceType.ListView:
                    return new ListViewSetting(parent, member);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static ObservableCollection<SettingField> GetEnumChoices(object parent, Type type)
        {
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
            return choices;
        }

        private ObservableCollection<SettingField> GetListChoices(object parent)
        {
            var choices = new ObservableCollection<SettingField>();
            PropertyInfo member = parent.GetType().GetProperty(ItemsSource);
            var list = member.GetValue(parent) as IEnumerable;
            if (list == null)
                throw new ArgumentException("ItemsSource must target an implementation of IEnumerable");
            foreach (var e in list)
            {
                if (!(e is string) && ItemsLabelPath == null)
                    throw new ArgumentException("ItemsSource elements must be strings unless ItemsLabelPath is declared");
                var field = e is string ? GetStringField(e) : GetField(e);
                choices.Add(field);
            }
            return choices;
        }

        private static SettingField GetStringField(object choice)
        {
            string name = choice as string;
            SettingField field = new SettingField(choice, name, name, null);
            return field;
        }

        private SettingField GetField(object choice)
        {
            Type type = choice.GetType();
            PropertyInfo member = type.GetProperty(ItemsLabelPath);
            string label = member.GetValue(choice) as string;
            if (label == null)
                throw new ArgumentException("ItemsLabelPath must target a string");
            SettingField field = new SettingField(choice, label, label, null);
            return field;
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
            e = new ConverterArgs(e, this);
            ButtonSetting element = new ButtonSetting(parent, member);
            Fill(element, member, e);
            element.Action = (Action) member.GetValue(parent);
            element.Alignment = Alignment;
            return element;
        }
    }
}
