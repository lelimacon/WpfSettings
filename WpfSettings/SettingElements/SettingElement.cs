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
            set { Set(ref _position, value); }
        }

        public string Label
        {
            get { return _label; }
            set { Set(ref _label, value); }
        }

        public int LabelWidth
        {
            get { return _labelWidth; }
            set { Set(ref _labelWidth, value); }
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

        protected void Set<T>(ref T variable, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, variable))
                return;
            variable = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Save();
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
            set { Set(ref _icon, value); }
        }

        public ObservableCollection<SettingSection> SubSections
        {
            get { return _subSections; }
            set { Set(ref _subSections, value); }
        }

        public ObservableCollection<SettingPageElement> Elements
        {
            get { return _elements; }
            set { Set(ref _elements, value); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(ref _isExpanded, value); }
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
            set { Set(ref _autoSave, value); }
        }

        public string Details
        {
            get { return _details; }
            set { Set(ref _details, value); }
        }

        protected SettingPageElement(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        protected void SetAndSave<T>(ref T variable, T value,
            [CallerMemberName] string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            Set(ref variable, value, propertyName);
            if (AutoSave)
                Save();
        }
    }

    internal class SettingGroup : SettingPageElement
    {
        private IEnumerable<SettingPageElement> _elements;

        public IEnumerable<SettingPageElement> Elements
        {
            get { return _elements; }
            set { Set(ref _elements, value); }
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
            set { SetAndSave(ref _value, value); }
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

    internal class NumberSetting : SettingPageElement
    {
        private int _value;
        private int _step;
        private int _maxValue;
        private int _minValue;

        public int Value
        {
            get { return _value; }
            set { SetAndSave(ref _value, value); }
        }

        public int Step
        {
            get { return _step; }
            set { Set(ref _step, value); }
        }

        public int MinValue
        {
            get { return _minValue; }
            set { Set(ref _minValue, value); }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set { Set(ref _maxValue, value); }
        }

        public NumberSetting(object parent, MemberInfo member)
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
                Value = (int) Member.GetValue(Parent);
        }
    }

    internal class TextSetting : SettingPageElement
    {
        private string _value;
        private int _height;

        public string Value
        {
            get { return _value; }
            set { SetAndSave(ref _value, value); }
        }

        public int Height
        {
            get { return _height; }
            set { Set(ref _height, value); }
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
            set { SetAndSave(ref _value, value); }
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

    internal class DateSetting : SettingPageElement
    {
        private DateTime _value;

        public DateTime Value
        {
            get { return _value; }
            set { SetAndSave(ref _value, value); }
        }

        public DateSetting(object parent, MemberInfo member)
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
                Value = (DateTime) Member.GetValue(Parent);
        }
    }

    internal class SettingField
    {
        public object Value { get; }
        public string Name { get; }
        public string Label { get; }
        public string Details { get; }

        public SettingField(object value, string name, string label, string details)
        {
            Value = value;
            Name = name;
            Label = label;
            Details = details;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    internal class ChoiceSetting : SettingPageElement
    {
        private ObservableCollection<SettingField> _choices;
        private SettingField _selectedValue;

        public ObservableCollection<SettingField> Choices
        {
            get { return _choices; }
            set { Set(ref _choices, value); }
        }

        public SettingField SelectedValue
        {
            get { return _selectedValue; }
            set { SetAndSave(ref _selectedValue, value); }
        }

        public ChoiceSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        public override void Save()
        {
            if (SelectedValue == null)
                return;
            Member.SetValue(Parent, SelectedValue.Value);
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
            {
                object value = Member.GetValue(Parent);
                SettingField field = Choices.FirstOrDefault(a => a.Value.Equals(value));
                SelectedValue = field;
            }
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
            OnSelectionCommand = new RelayCommand<SettingField>(ChangeSelection);
            GroupName = Member.Name + _id++;
        }

        public void ChangeSelection(SettingField field)
        {
            SelectedValue = field;
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
