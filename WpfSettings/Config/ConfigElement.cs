using System.Collections.ObjectModel;
using System.Reflection;

namespace WpfSettings.Config
{
    public abstract class ConfigElement
    {
        public object Parent { get; }
        public PropertyInfo Property { get; }
        public string Label { get; set; }
        public string Image { get; set; }
        public string Details { get; set; }

        protected ConfigElement(object parent, PropertyInfo property)
        {
            Parent = parent;
            Property = property;
            Label = property?.Name;
        }
    }

    public class ConfigSection : ConfigElement
    {
        public ObservableCollection<ConfigSection> SubSections { get; set; }
        public ObservableCollection<ConfigPageElement> Elements { get; set; }

        public ConfigSection(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }
    }

    public abstract class ConfigPageElement : ConfigElement
    {
        protected ConfigPageElement(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }
    }

    public class ConfigGroup : ConfigPageElement
    {
        public ObservableCollection<ConfigPageElement> Elements { get; }

        public ConfigGroup(object parent, PropertyInfo property)
            : base(parent, property)
        {
            Elements = new ObservableCollection<ConfigPageElement>();
        }

        public void Add(ConfigPageElement element)
        {
            Elements.Add(element);
        }
    }

    public class StringConfig : ConfigPageElement
    {
        public string Value { get; set; }

        public StringConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }
    }

    public class TextConfig : ConfigPageElement
    {
        public string Value { get; set; }
        public int Height { get; set; }

        public TextConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }
    }

    public class BoolConfig : ConfigPageElement
    {
        public bool Value { get; set; }

        public BoolConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }
    }

    public class ChoiceConfig : ConfigPageElement
    {
        public ObservableCollection<string> Choices { get; set; }
        public int SelectedIndex { get; set; }

        public ChoiceConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }
    }
}
