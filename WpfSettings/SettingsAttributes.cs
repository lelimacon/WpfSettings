using System;

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
    }

    public class SettingGroupAttribute : SettingAttribute
    {
    }

    public class SettingTextAttribute : SettingAttribute
    {
        private int _height;

        public string Details { get; set; }

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
    }

    public class SettingStringAttribute : SettingAttribute
    {
        public string Details { get; set; }
    }

    public class SettingBoolAttribute : SettingAttribute
    {
        public string Details { get; set; }
    }

    public enum ChoiceType
    {
        DropDown,
        RadioButtons
    }

    public class SettingChoiceAttribute : SettingAttribute
    {
        public string Details { get; set; }
        public string GroupName { get; set; }
        public ChoiceType Type { get; set; } = ChoiceType.DropDown;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingFieldAttribute : SettingAttribute
    {
    }

    public class SettingButtonAttribute : SettingAttribute
    {
        public string Details { get; set; }
    }
}
