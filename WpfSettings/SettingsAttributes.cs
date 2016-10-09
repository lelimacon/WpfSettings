using System;

namespace WpfSettings
{
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

    [AttributeUsage(AttributeTargets.Class)]
    public class SettingSectionAttribute : SettingAttribute
    {
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

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingTextAttribute : SettingAttribute
    {
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

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingStringAttribute : SettingAttribute
    {
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

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingBoolAttribute : SettingAttribute
    {
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

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingChoiceAttribute : SettingAttribute
    {
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
