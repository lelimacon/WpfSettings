using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfSettings.Annotations;
using WpfSettings.Utils;

namespace WpfSettings.Config
{
    public abstract class ConfigElement : INotifyPropertyChanged
    {
        private int _position;
        private string _label;
        public object Parent { get; }
        public MemberInfo Member { get; }

        public int Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                _position = value;
                OnPropertyChanged();
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                if (value == _label) return;
                _label = value;
                OnPropertyChanged();
            }
        }

        protected ConfigElement(object parent, MemberInfo member)
        {
            Parent = parent;
            Member = member;
            Label = member?.Name;
            var propertyChanged = parent as INotifyPropertyChanged;
            if (propertyChanged != null)
                propertyChanged.PropertyChanged += OuterPropertyChanged;
        }

        public virtual void Save()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected abstract void OuterPropertyChanged(object sender, PropertyChangedEventArgs e);
    }

    public class ConfigSection : ConfigElement
    {
        private BitmapSource _image;
        private ObservableCollection<ConfigSection> _subSections;
        private ObservableCollection<ConfigPageElement> _elements;

        public BitmapSource Image
        {
            get { return _image; }
            set
            {
                if (Equals(value, _image)) return;
                _image = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ConfigSection> SubSections
        {
            get { return _subSections; }
            set
            {
                if (Equals(value, _subSections)) return;
                _subSections = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ConfigPageElement> Elements
        {
            get { return _elements; }
            set
            {
                if (Equals(value, _elements)) return;
                _elements = value;
                OnPropertyChanged();
            }
        }

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

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }

    public abstract class ConfigPageElement : ConfigElement
    {
        private bool _autoSave;

        public bool AutoSave
        {
            get { return _autoSave; }
            set
            {
                if (value == _autoSave) return;
                _autoSave = value;
                OnPropertyChanged();
            }
        }

        protected ConfigPageElement(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    public class ConfigGroup : ConfigPageElement
    {
        private IEnumerable<ConfigPageElement> _elements;

        public IEnumerable<ConfigPageElement> Elements
        {
            get { return _elements; }
            set
            {
                if (Equals(value, _elements)) return;
                _elements = value;
                OnPropertyChanged();
            }
        }

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

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }

    public class StringConfig : ConfigPageElement
    {
        private string _value;
        private string _details;

        public string Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
                if (AutoSave) Save();
            }
        }

        public string Details
        {
            get { return _details; }
            set
            {
                if (value == _details) return;
                _details = value;
                OnPropertyChanged();
            }
        }

        public StringConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            Member.SetValue(Parent, Value);
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = (string) Member.GetValue(Parent);
        }
    }

    public class TextConfig : ConfigPageElement
    {
        private string _value;
        private string _details;
        private int _height;

        public string Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
                if (AutoSave) Save();
            }
        }

        public string Details
        {
            get { return _details; }
            set
            {
                if (value == _details) return;
                _details = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                if (value == _height) return;
                _height = value;
                OnPropertyChanged();
            }
        }

        public TextConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
            Height = 60;
        }

        public override void Save()
        {
            Member.SetValue(Parent, Value);
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = (string) Member.GetValue(Parent);
        }
    }

    public class BoolConfig : ConfigPageElement
    {
        private bool _value;
        private string _details;

        public bool Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
                if (AutoSave) Save();
            }
        }

        public string Details
        {
            get { return _details; }
            set
            {
                if (value == _details) return;
                _details = value;
                OnPropertyChanged();
            }
        }

        public BoolConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            Member.SetValue(Parent, Value);
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = (bool) Member.GetValue(Parent);
        }
    }

    public class ChoiceConfig : ConfigPageElement
    {
        private ObservableCollection<string> _choices;
        private string _selectedValue;
        private string _details;

        public ObservableCollection<string> Choices
        {
            get { return _choices; }
            set
            {
                if (Equals(value, _choices)) return;
                _choices = value;
                OnPropertyChanged();
            }
        }

        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (value == _selectedValue) return;
                _selectedValue = value;
                OnPropertyChanged();
                if (AutoSave) Save();
            }
        }

        public string Details
        {
            get { return _details; }
            set
            {
                if (value == _details) return;
                _details = value;
                OnPropertyChanged();
            }
        }

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

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
            {
                Type type = Member.GetValueType();
                string enumValue = GetFieldLabel(type, Member.GetValue(Parent).ToString());
                SelectedValue = enumValue;
            }
        }

        private static string GetFieldLabel(Type enumType, string fieldName)
        {
            var memberInfos = enumType.GetMember(fieldName);
            var attr = memberInfos[0].GetCustomAttribute<SettingFieldAttribute>(false);
            return attr?.Label;
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
        public ICommand OnSelectionCommand { get; }

        public RadioButtonsConfig(object parent, MemberInfo member)
            : base(parent, member)
        {
            OnSelectionCommand = new RelayCommand<string>(ChangeSelection);
            GroupName = Member.Name + _id++;
        }

        public void ChangeSelection(string selection)
        {
            SelectedValue = selection;
        }
    }
}
