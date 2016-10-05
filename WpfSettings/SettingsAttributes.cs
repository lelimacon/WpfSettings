using System;

namespace WpfSettings
{
    public abstract class SettingAttribute : Attribute
    {
        public string Label { get; set; }

        protected SettingAttribute()
        {
        }

        protected SettingAttribute(string label)
        {
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
    }
}
