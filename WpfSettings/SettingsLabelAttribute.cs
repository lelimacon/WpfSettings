using System;

namespace WpfSettings
{
    public class SettingsLabelAttribute : Attribute
    {
        public string Label { get; private set; }

        public SettingsLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
