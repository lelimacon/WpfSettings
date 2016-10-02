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

    public class SettingFieldAttribute : SettingAttribute
    {
        public object DefaultValue { get; set; }

        public SettingFieldAttribute()
        {
        }

        public SettingFieldAttribute(string label)
            : base(label)
        {
        }
    }
}
