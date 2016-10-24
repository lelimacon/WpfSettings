using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfSettings.Annotations;
using WpfSettings.Utils.Reflection;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.SettingElements
{
    public abstract class SettingElement : INotifyPropertyChanged
    {
        private int _position;
        private string _label;
        private int _labelWidth;
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

        public int LabelWidth
        {
            get { return _labelWidth; }
            set
            {
                if (value == _labelWidth) return;
                _labelWidth = value;
                OnPropertyChanged();
            }
        }

        protected SettingElement(object parent, MemberInfo member)
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

    public class SettingSection : SettingElement
    {
        private BitmapSource _icon;
        private ObservableCollection<SettingSection> _subSections;
        private ObservableCollection<SettingPageElement> _elements;
        private bool _isExpanded;

        public BitmapSource Icon
        {
            get { return _icon; }
            set
            {
                if (Equals(value, _icon)) return;
                _icon = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SettingSection> SubSections
        {
            get { return _subSections; }
            set
            {
                if (Equals(value, _subSections)) return;
                _subSections = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SettingPageElement> Elements
        {
            get { return _elements; }
            set
            {
                if (Equals(value, _elements)) return;
                _elements = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public SettingSection(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            foreach (SettingSection section in SubSections)
                section.Save();
            foreach (SettingPageElement element in Elements)
                element.Save();
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }

    public abstract class SettingPageElement : SettingElement
    {
        private bool _autoSave;
        private string _details;

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

        protected SettingPageElement(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    internal class SettingGroup : SettingPageElement
    {
        private IEnumerable<SettingPageElement> _elements;

        public IEnumerable<SettingPageElement> Elements
        {
            get { return _elements; }
            set
            {
                if (Equals(value, _elements)) return;
                _elements = value;
                OnPropertyChanged();
            }
        }

        public SettingGroup(object parent, MemberInfo member, IEnumerable<SettingPageElement> elements)
            : base(parent, member)
        {
            Elements = elements;
        }

        public override void Save()
        {
            foreach (SettingPageElement element in Elements)
                element.Save();
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }

    internal class StringSetting : SettingPageElement
    {
        private string _value;

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

        public StringSetting(object parent, MemberInfo member)
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

    internal class TextSetting : SettingPageElement
    {
        private string _value;
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

        public TextSetting(object parent, MemberInfo member)
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

    internal class BoolSetting : SettingPageElement
    {
        private bool _value;

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

        public BoolSetting(object parent, MemberInfo member)
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

    internal class ChoiceSetting : SettingPageElement
    {
        private ObservableCollection<string> _choices;
        private string _selectedValue;

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


        public ChoiceSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            string value = SelectedValue;
            if (value == null)
                return;
            Type type = Member.GetValueType();
            string[] names = Enum.GetNames(type);
            string name = names.First(n => FieldLabel(type, n) == value);
            var entry = Enum.Parse(type, name);
            Member.SetValue(Parent, entry);
        }

        private static string FieldLabel(Type type, string name)
        {
            MemberInfo info = type.GetMember(name)[0];
            var attribute = info.GetCustomAttribute<SettingFieldAttribute>(false);
            return attribute?.Label ?? SettingsConverter.InferLabel(name);
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

    internal class DropDownSetting : ChoiceSetting
    {
        public DropDownSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    internal class RadioButtonsSetting : ChoiceSetting
    {
        private static int _id;
        public string GroupName { get; }
        public ICommand OnSelectionCommand { get; }

        public RadioButtonsSetting(object parent, MemberInfo member)
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

    internal class ButtonSetting : SettingPageElement
    {
        public Action Action { get; set; }
        public string GetAction { get; set; }
        public ICommand PressedCommand { get; set; }
        public HorizontalAlignment Alignment { get; set; }

        public ButtonSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
            PressedCommand = new RelayCommand(() => { Action(); });
            Alignment = HorizontalAlignment.Left;
        }

        public override void Save()
        {
            Member.SetValue(Parent, Action);
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Action = (Action) Member.GetValue(Parent);
        }
    }
}
