namespace WpfSettings.Config
{
    internal class ConverterArgs
    {
        public bool AutoSave { get; set; }
        public int LabelWidth { get; set; }
        public SectionExpansion Expansion { get; set; }

        public ConverterArgs()
        {
            AutoSave = true;
            LabelWidth = 140;
            Expansion = SectionExpansion.Unset;
        }

        public ConverterArgs(ConverterArgs other)
        {
            AutoSave = other.AutoSave;
            LabelWidth = other.LabelWidth;
            Expansion = other.Expansion;
        }

        public ConverterArgs Integrate(SettingAttribute attribute)
        {
            var e = new ConverterArgs(this);
            if (attribute.LabelWidth > 0)
                e.LabelWidth = attribute.LabelWidth;
            return e;
        }

        public ConverterArgs Integrate(SettingSectionAttribute attribute)
        {
            var e = new ConverterArgs(this);
            if (attribute.LabelWidth > 0)
                e.LabelWidth = attribute.LabelWidth;
            if (attribute.Expansion != SectionExpansion.Unset)
                e.Expansion = attribute.Expansion;
            return e;
        }

        public ConverterArgs NextArgs(SettingSectionAttribute attribute)
        {
            var e = new ConverterArgs(this)
            {
                LabelWidth = attribute.LabelWidth,
                Expansion = NextExpansion()
            };
            return e;
        }

        public ConverterArgs NextArgs(SettingGroupAttribute attribute)
        {
            var e = new ConverterArgs(this)
            {
                LabelWidth = attribute.LabelWidth
            };
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
