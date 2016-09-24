using System.Collections.ObjectModel;

namespace WpfSettings
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
        public ObservableCollection<ConfigSection> SubSections { get; }
        public ObservableCollection<ConfigPageElement> Elements { get; }

        public ConfigSection(string id, string label)
            : base(id, label)
        {
            SubSections = new ObservableCollection<ConfigSection>();
            Elements = new ObservableCollection<ConfigPageElement>();
        }

        public void Add(ConfigSection element)
        {
            SubSections.Add(element);
        }

        public void Add(ConfigPageElement element)
        {
            Elements.Add(element);
        }
    }

    public class ConfigGroup : ConfigPageElement
    {
        public ObservableCollection<ConfigPageElement> Elements { get; }

        public ConfigGroup(string id, string label)
            : base(id, label)
        {
            Elements = new ObservableCollection<ConfigPageElement>();
        }

        public void Add(ConfigPageElement element)
        {
            Elements.Add(element);
        }
    }

    public class ConfigPageElement : ConfigElement
    {
        public ConfigPageElement(string id, string label)
            : base(id, label)
        {
        }
    }

    /*
    internal class TitleConfig : ConfigSimpleElement
    {
        public TitleConfig(string id, string label)
            : base(id, label)
        {
        }
    }
    */

    public class StringConfig : ConfigPageElement
    {
        public string Value { get; set; }

        public StringConfig(string id, string label)
            : base(id, label)
        {
        }
    }

    public class TextConfig : ConfigPageElement
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

    public class BoolConfig : ConfigPageElement
    {
        public bool Value { get; set; }

        public BoolConfig(string id, string label)
            : base(id, label)
        {
            Value = true;
        }
    }

    public class ChoiceConfig : ConfigPageElement
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
