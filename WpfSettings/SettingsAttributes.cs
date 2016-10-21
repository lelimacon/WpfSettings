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

        protected SettingAttribute()
        {
            Position = 0;
        }

        protected SettingAttribute(string label)
        {
            Position = 0;
            Label = label;
        }

        protected SettingAttribute(int position, string label)
        {
            Position = position;
            Label = label;
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

        public SettingSectionAttribute()
        {
        }

        public SettingSectionAttribute(string label)
            : base(label)
        {
        }

        public SettingSectionAttribute(int position, string label)
            : base(position, label)
        {
        }
    }

    public class SettingGroupAttribute : SettingAttribute
    {
        public SettingGroupAttribute()
        {
        }

        public SettingGroupAttribute(string label)
            : base(label)
        {
        }

        public SettingGroupAttribute(int position, string label)
            : base(position, label)
        {
        }
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

        public SettingTextAttribute()
        {
        }

        public SettingTextAttribute(string label)
            : base(label)
        {
        }

        public SettingTextAttribute(int position, string label)
            : base(position, label)
        {
        }
    }

    public class SettingStringAttribute : SettingAttribute
    {
        public string Details { get; set; }

        public SettingStringAttribute()
        {
        }

        public SettingStringAttribute(string label)
            : base(label)
        {
        }

        public SettingStringAttribute(int position, string label)
            : base(position, label)
        {
        }
    }

    public class SettingBoolAttribute : SettingAttribute
    {
        public string Details { get; set; }

        public SettingBoolAttribute()
        {
        }

        public SettingBoolAttribute(string label)
            : base(label)
        {
        }

        public SettingBoolAttribute(int position, string label)
            : base(position, label)
        {
        }
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

        public SettingChoiceAttribute()
        {
        }

        public SettingChoiceAttribute(string label)
            : base(label)
        {
        }

        public SettingChoiceAttribute(int position, string label)
            : base(position, label)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingFieldAttribute : SettingAttribute
    {
        public SettingFieldAttribute()
        {
        }

        public SettingFieldAttribute(string label)
            : base(label)
        {
        }

        public SettingFieldAttribute(int position, string label)
            : base(position, label)
        {
        }
    }

    public class SettingButtonAttribute : SettingAttribute
    {
        public string Details { get; set; }

        public SettingButtonAttribute()
        {
        }

        public SettingButtonAttribute(string label)
            : base(label)
        {
        }

        public SettingButtonAttribute(int position, string label)
            : base(position, label)
        {
        }
    }
}
