namespace WpfSettings.Config
{
    internal class ConverterArgs
    {
        private int _labelWidth;
        public bool AutoSave { get; set; }

        public int LabelWidth
        {
            get { return _labelWidth; }
            set
            {
                if (value > 0)
                    _labelWidth = value;
            }
        }

        public ConverterArgs()
        {
            AutoSave = true;
            LabelWidth = 140;
        }

        public ConverterArgs(ConverterArgs other)
        {
            AutoSave = other.AutoSave;
            LabelWidth = other.LabelWidth;
        }
    }
}
