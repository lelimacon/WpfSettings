using System;

namespace WpfSettings.SettingElements
{
    internal class ConverterArgs
    {
        public bool AutoSave { get; set; }
        public string LabelWidth { get; set; }
        public SectionExpansion Expansion { get; set; }
        public Action<string> SelectSection { get; set; }

        public ConverterArgs()
        {
            AutoSave = true;
            LabelWidth = "140";
            Expansion = SectionExpansion.Unset;
            SelectSection = null;
        }

        public ConverterArgs(ConverterArgs other)
        {
            AutoSave = other.AutoSave;
            LabelWidth = other.LabelWidth;
            Expansion = other.Expansion;
            SelectSection = other.SelectSection;
        }

        public ConverterArgs(ConverterArgs other, SettingAttribute attribute)
        {
            AutoSave = other.AutoSave;
            LabelWidth = !string.IsNullOrEmpty(attribute.LabelWidth) ? attribute.LabelWidth : other.LabelWidth;
            Expansion = other.Expansion;
            SelectSection = other.SelectSection;
        }

        public ConverterArgs(ConverterArgs other, SettingSectionAttribute attribute)
        {
            AutoSave = other.AutoSave;
            LabelWidth = !string.IsNullOrEmpty(attribute.LabelWidth) ? attribute.LabelWidth : other.LabelWidth;
            Expansion = attribute.Expansion != SectionExpansion.Unset
                ? attribute.Expansion
                : other.Expansion;
            SelectSection = other.SelectSection;
        }

        public ConverterArgs ChildrenArgs(SettingSectionAttribute attribute)
        {
            var e = new ConverterArgs(this);
            if (!string.IsNullOrEmpty(attribute.LabelWidth))
                e.LabelWidth = attribute.LabelWidth;
            e.Expansion = attribute.Expansion != SectionExpansion.Unset
                ? attribute.Expansion
                : NextExpansion();
            return e;
        }

        public ConverterArgs ChildrenArgs(SettingGroupAttribute attribute)
        {
            var e = new ConverterArgs(this);
            if (!string.IsNullOrEmpty(attribute.LabelWidth))
                e.LabelWidth = attribute.LabelWidth;
            return e;
        }

        private SectionExpansion NextExpansion()
        {
            switch (Expansion)
            {
                case SectionExpansion.Expanded:
                    return SectionExpansion.Closed;
                case SectionExpansion.ChildrenExpanded:
                    return SectionExpansion.Expanded;
                case SectionExpansion.ChildrenExpandedRecursive:
                    return SectionExpansion.ExpandedRecursive;
                default:
                    return Expansion;
            }
        }
    }
}
