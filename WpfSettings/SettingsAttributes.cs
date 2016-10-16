using System;

namespace WpfSettings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class SettingAttribute : Attribute
    {
        public int Position { get; set; }
        public string Label { get; set; }
        
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

    public class SettingSectionAttribute : SettingAttribute
    {
        public string Image { get; set; }

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
        public string Details { get; set; }

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
}
