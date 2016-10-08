using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using WpfSettings.Utils;

namespace WpfSettings.Config
{
    public abstract class ConfigElement
    {
        public object Parent { get; }
        public PropertyInfo Property { get; }
        public string Label { get; set; }
        public string Details { get; set; }

        protected ConfigElement(object parent, PropertyInfo property)
        {
            Parent = parent;
            Property = property;
            Label = property?.Name;
        }

        public virtual void Save()
        {
        }
    }

    public class ConfigSection : ConfigElement
    {
        public string Image { get; set; }
        public ObservableCollection<ConfigSection> SubSections { get; set; }
        public ObservableCollection<ConfigPageElement> Elements { get; set; }

        public ConfigSection(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }

        public override void Save()
        {
            foreach (ConfigSection section in SubSections)
                section.Save();
            foreach (ConfigPageElement element in Elements)
                element.Save();
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

        public override void Save()
        {
            foreach (ConfigPageElement element in Elements)
                element.Save();
        }
    }

    public class StringConfig : ConfigPageElement
    {
        public string Value { get; set; }

        public StringConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }

        public override void Save()
        {
            Property.SetValue(Parent, Value);
        }
    }

    public class TextConfig : ConfigPageElement
    {
        public string Value { get; set; }
        public int Height { get; set; }

        public TextConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
            Height = 60;
        }

        public override void Save()
        {
            Property.SetValue(Parent, Value);
        }
    }

    public class BoolConfig : ConfigPageElement
    {
        public bool Value { get; set; }

        public BoolConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }

        public override void Save()
        {
            Property.SetValue(Parent, Value);
        }
    }

    public class ChoiceConfig : ConfigPageElement
    {
        public ObservableCollection<string> Choices { get; set; }
        public string SelectedValue { get; set; }

        public ChoiceConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }

        public override void Save()
        {
            string value = SelectedValue;
            Type type = Property.PropertyType;
            string name = Enum.GetNames(type).First(n => FieldLabel(type, n) == value);
            var entry = Enum.Parse(type, name);
            Property.SetValue(Parent, entry);
        }

        private static string FieldLabel(Type type, string n)
        {
            MemberInfo info = type.GetMember(n)[0];
            var attribute = info.GetCustomAttribute<SettingFieldAttribute>(false);
            return attribute?.Label;
        }
    }

    public class DropDownConfig : ChoiceConfig
    {
        public DropDownConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
        }
    }

    public class RadioButtonsConfig : ChoiceConfig
    {
        public string Id { get; }
        public ICommand OnSelectionCommand => new RelayCommand<string>(ChangeSelection);

        public RadioButtonsConfig(object parent, PropertyInfo property)
            : base(parent, property)
        {
            Id = property.Name;
        }

        public void ChangeSelection(string selection)
        {
            SelectedValue = selection;
        }
    }
}
