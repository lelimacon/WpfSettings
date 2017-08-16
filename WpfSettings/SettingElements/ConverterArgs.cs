using System;

namespace WpfSettings.SettingElements
{
    public class ConverterArgs
    {
        public ReadOnly IsReadOnly { get; set; }
        public bool AutoSave { get; set; }
        public string LabelWidth { get; set; }
        public SectionExpansion Expansion { get; set; }
        public Action<string> SelectSection { get; set; }
        public string Path { get; private set; }

        public ConverterArgs()
        {
            IsReadOnly = ReadOnly.Unset;
            AutoSave = true;
            LabelWidth = "140";
            Expansion = SectionExpansion.Unset;
            SelectSection = null;
            Path = string.Empty;
        }

        public ConverterArgs(ConverterArgs other)
        {
            IsReadOnly = other.IsReadOnly;
            AutoSave = other.AutoSave;
            LabelWidth = other.LabelWidth;
            Expansion = other.Expansion;
            SelectSection = other.SelectSection;
            Path = other.Path;
        }

        public ConverterArgs(ConverterArgs other, SettingSectionAttribute attribute)
        {
            IsReadOnly = other.IsReadOnly == ReadOnly.Unset ? attribute.IsReadOnly : other.IsReadOnly;
            AutoSave = other.AutoSave;
            LabelWidth = !string.IsNullOrEmpty(attribute.LabelWidth) ? attribute.LabelWidth : other.LabelWidth;
            Expansion = attribute.Expansion != SectionExpansion.Unset ? attribute.Expansion : other.Expansion;
            SelectSection = other.SelectSection;
            Path = other.Path;
        }

        public ConverterArgs(ConverterArgs other, SettingAttribute attribute)
        {
            IsReadOnly = other.IsReadOnly == ReadOnly.Unset ? attribute.IsReadOnly : other.IsReadOnly;
            AutoSave = other.AutoSave;
            LabelWidth = !string.IsNullOrEmpty(attribute.LabelWidth) ? attribute.LabelWidth : other.LabelWidth;
            Expansion = other.Expansion;
            SelectSection = other.SelectSection;
            Path = other.Path;
        }

        public ConverterArgs ChildrenArgs(SettingSectionAttribute attribute, string name)
        {
            var e = new ConverterArgs(this);
            e.IsReadOnly = attribute.IsReadOnly == ReadOnly.Unset ? attribute.IsReadOnly : NextReadOnly();
            if (!string.IsNullOrEmpty(attribute.LabelWidth))
                e.LabelWidth = attribute.LabelWidth;
            e.Expansion = attribute.Expansion != SectionExpansion.Unset ? attribute.Expansion : NextExpansion();
            e.Path += (e.Path == "" ? "" : ".") + name;
            return e;
        }

        private ReadOnly NextReadOnly()
        {
            switch (IsReadOnly)
            {
                case ReadOnly.Yes:
                    return ReadOnly.No;
                default:
                    return IsReadOnly;
            }
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
