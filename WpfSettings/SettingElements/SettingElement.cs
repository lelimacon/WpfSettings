using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfSettings.Annotations;
using WpfSettings.Controls;
using WpfSettings.Utils.Reflection;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.SettingElements
{
    public abstract class SettingElement : INotifyPropertyChanged
    {
        private int _position;
        private string _label;
        public object Parent { get; }
        public MemberInfo Member { get; }
        public string Name => Member.Name;

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
        private bool _isSelected;

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

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
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

    public abstract class SettingPageElement : SettingElement, IListElement
    {
        private string _height;
        private string _labelWidth;
        private bool _autoSave;
        private string _details;
        private string _prefix;
        private string _suffix;
        private string _suffixLabel;

        public string Height
        {
            get { return _height; }
            set { Set(ref _height, value); }
        }

        public string LabelWidth
        {
            get { return _labelWidth; }
            set { Set(ref _labelWidth, value); }
        }

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

        public string Prefix
        {
            get { return _prefix; }
            set { Set(ref _prefix, value); }
        }

        public string Suffix
        {
            get { return _suffix; }
            set { Set(ref _suffix, value); }
        }

        public string SuffixLabel
        {
            get { return _suffixLabel; }
            set { Set(ref _suffixLabel, value); }
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

    internal abstract class SettingGroup : SettingPageElement
    {
        private ObservableCollection<SettingPageElement> _elements;

        public ObservableCollection<SettingPageElement> Elements
        {
            get { return _elements; }
            set { Set(ref _elements, value); }
        }

        protected SettingGroup(object parent, MemberInfo member)
            : base(parent, member)
        {
            Elements = new ObservableCollection<SettingPageElement>();
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

    internal class SettingGroupBox : SettingGroup
    {
        public SettingGroupBox(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    internal class SettingGroupTitle : SettingGroup
    {
        public SettingGroupTitle(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    internal class StringSetting : SettingPageElement
    {
        private string _value;
        private char _separator;

        public string Value
        {
            get { return _value; }
            set { SetAndSave(ref _value, value); }
        }

        public char Separator
        {
            get { return _separator; }
            set { Set(ref _separator, value); }
        }

        public StringSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
            Separator = ';';
        }

        public override void Save()
        {
            if (Member.DeclaringType == typeof(string))
                Member.SetValue(Parent, Value);
            else if (Member.DeclaringType == typeof(string[]))
            {
                string[] values = Value.Split(Separator);
                Member.SetValue(Parent, values);
            }
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = GetValue();
        }

        private string GetValue()
        {
            Type type = Member.GetValueType();
            if (type == typeof(string))
                return (string) Member.GetValue(Parent);
            // type == typeof(string[])
            return string.Join(Separator.ToString(), (string[]) Member.GetValue(Parent));
        }
    }

    public enum NumberSettingType
    {
        Spinner,
        Slider,
        SliderAndSpinner,
        SliderAndValue
    }

    internal class NumberSetting : SettingPageElement
    {
        private NumberSettingType _type;
        private int _value;
        private int _maxValue;
        private int _minValue;
        private int _step;
        private int _tickFrequency;

        public bool SliderVisible => Type != NumberSettingType.Spinner;
        public bool SpinnerVisible => Type == NumberSettingType.Spinner || Type == NumberSettingType.SliderAndSpinner;
        public bool LabelVisible => Type == NumberSettingType.SliderAndValue;
        public string SliderWidth => SliderVisible ? "3*" : "0";
        public string SpinnerWidth => SpinnerVisible ? "*" : LabelVisible ? "Auto" : "0";
        public bool SnapToTick => TickFrequency > 0;

        public NumberSettingType Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }

        public int Value
        {
            get { return _value; }
            set { SetAndSave(ref _value, value); }
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

        public int Step
        {
            get { return _step; }
            set { Set(ref _step, value); }
        }

        public int TickFrequency
        {
            get { return _tickFrequency; }
            set { Set(ref _tickFrequency, value); }
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

        public string Value
        {
            get { return _value; }
            set { SetAndSave(ref _value, value); }
        }

        public TextSetting(object parent, MemberInfo member)
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

    internal abstract class ChoiceSetting : SettingPageElement
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

        protected ChoiceSetting(object parent, MemberInfo member)
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

    internal class ListViewSetting : ChoiceSetting
    {
        public bool SearchBox { get; set; }

        public string SearchBoxHeight => SearchBox ? "24" : "0";

        public ListViewSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
        }
    }

    public enum ButtonType
    {
        Normal,
        Flat,
        Link
    }

    internal abstract class ButtonSetting : SettingPageElement
    {
        private HorizontalAlignment _alignment;
        private ButtonType _type;

        public HorizontalAlignment Alignment
        {
            get { return _alignment; }
            set { Set(ref _alignment, value); }
        }

        public ButtonType Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }

        public ICommand PressedCommand { get; set; }

        protected ButtonSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
            Alignment = HorizontalAlignment.Left;
            PressedCommand = new RelayCommand(OnPressed);
        }

        protected abstract void OnPressed();

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }

    internal class LinkButtonSetting : ButtonSetting
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { SetAndSave(ref _path, value); }
        }

        public Action<string> SelectSection { get; set; }

        public LinkButtonSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        protected override void OnPressed()
        {
            SelectSection?.Invoke(Path);
        }

        public override void Save()
        {
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OuterPropertyChanged(sender, e);
            if (e.PropertyName == Member.Name)
                Path = (string) Member.GetValue(Parent);
        }
    }

    internal class CustomButtonSetting : ButtonSetting
    {
        private Action _action;

        public Action Action
        {
            get { return _action; }
            set { SetAndSave(ref _action, value); }
        }


        public CustomButtonSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
        }

        protected override void OnPressed()
        {
            Action();
        }

        public override void Save()
        {
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OuterPropertyChanged(sender, e);
            if (e.PropertyName == Member.Name)
                Action = (Action) Member.GetValue(Parent);
        }
    }
}
