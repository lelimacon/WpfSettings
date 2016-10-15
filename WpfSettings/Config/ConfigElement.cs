using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfSettings.Utils;

namespace WpfSettings.Config
{
    public abstract class ConfigElement
    {
        public object Parent { get; }
        public MemberInfo Member { get; }
        public int Position { get; set; }
        public string Label { get; set; }

        protected ConfigElement(object parent, MemberInfo member)
        {
            Parent = parent;
            Member = member;
            Label = member?.Name;
        }

        public virtual void Save()
        {
        }
    }

    public class ConfigSection : ConfigElement
    {
        public BitmapSource Image { get; set; }
        public ObservableCollection<ConfigSection> SubSections { get; set; }
        public ObservableCollection<ConfigPageElement> Elements { get; set; }

        public ConfigSection(object parent, MemberInfo member)
            : base(parent, member)
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
        protected ConfigPageElement(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    public class ConfigGroup : ConfigPageElement
    {
        public IEnumerable<ConfigPageElement> Elements { get; set; }

        public ConfigGroup(object parent, MemberInfo member, IEnumerable<ConfigPageElement> elements)
            : base(parent, member)
        {
            Elements = elements;
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
        public string Details { get; set; }

        public StringConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            Member.SetValue(Parent, Value);
        }
    }

    public class TextConfig : ConfigPageElement
    {
        public string Value { get; set; }
        public string Details { get; set; }
        public int Height { get; set; }

        public TextConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
            Height = 60;
        }

        public override void Save()
        {
            Member.SetValue(Parent, Value);
        }
    }

    public class BoolConfig : ConfigPageElement
    {
        public bool Value { get; set; }
        public string Details { get; set; }

        public BoolConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            Member.SetValue(Parent, Value);
        }
    }

    public class ChoiceConfig : ConfigPageElement
    {
        public ObservableCollection<string> Choices { get; set; }
        public string SelectedValue { get; set; }
        public string Details { get; set; }

        public ChoiceConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            string value = SelectedValue;
            Type type = Member.GetValueType();
            string name = Enum.GetNames(type).First(n => FieldLabel(type, n) == value);
            var entry = Enum.Parse(type, name);
            Member.SetValue(Parent, entry);
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
        public DropDownConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    public class RadioButtonsConfig : ChoiceConfig
    {
        private static int _id = 0;
        public string GroupName { get; }
        public ICommand OnSelectionCommand => new RelayCommand<string>(ChangeSelection);

        public RadioButtonsConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
            GroupName = Member.Name + _id++;
        }

        public void ChangeSelection(string selection)
        {
            SelectedValue = selection;
        }
    }
}
