using System.Collections.ObjectModel;

namespace WpfSettingsControl
{
    public abstract class ConfigElement
    {
        public string Id { get; private set; }
        public string Label { get; set; }
        public string Image { get; set; }
        public string Details { get; set; }

        protected ConfigElement(string id, string label)
        {
            Id = id;
            Label = label;
        }
    }

    public class ConfigSection : ConfigElement
    {
        public ObservableCollection<ConfigSection> SubSections { get; private set; }
        public ObservableCollection<ConfigSimpleElement> Elements { get; private set; }
        public ConfigSection(string id, string label)
            : base(id, label)
        {
            SubSections = new ObservableCollection<ConfigSection>();
            Elements = new ObservableCollection<ConfigSimpleElement>();
        }
    }

    public class ConfigSimpleElement : ConfigElement
    {
        public ConfigSimpleElement(string id, string label)
            : base(id, label)
        {
        }
    }

    public class ConfigGroup : ConfigSimpleElement
    {
        public ObservableCollection<ConfigSimpleElement> Elements { get; private set; }
        public ConfigGroup(string id, string label)
            : base(id, label)
        {
            Elements = new ObservableCollection<ConfigSimpleElement>();
        }
    }

    /*
    public class TitleConfig : ConfigSimpleElement
    {
        public TitleConfig(string id, string label)
            : base(id, label)
        {
        }
    }
    */

    public class StringConfig : ConfigSimpleElement
    {
        public string Value { get; set; }

        public StringConfig(string id, string label)
            : base(id, label)
        {
        }
    }

    public class TextConfig : ConfigSimpleElement
    {
        public string Value { get; set; }
        public int Height { get; set; }

        public TextConfig(string id, string label)
            : this(id, label, 60)
        {
        }

        public TextConfig(string id, string label, int height)
            : base(id, label)
        {
            Height = height;
        }
    }

    public class BoolConfig : ConfigSimpleElement
    {
        public bool Value { get; set; }

        public BoolConfig(string id, string label)
            : base(id, label)
        {
        }
    }

    public class ChoiceConfig : ConfigSimpleElement
    {
        public ObservableCollection<string> Choices { get; private set; }
        public int SelectedIndex { get; set; }

        public ChoiceConfig(string id, string label)
            : this(id, label, new ObservableCollection<string>())
        {
        }

        public ChoiceConfig(string id, string label, ObservableCollection<string> choices)
            : base(id, label)
        {
            Choices = choices;
            SelectedIndex = 0;
        }
    }
}
