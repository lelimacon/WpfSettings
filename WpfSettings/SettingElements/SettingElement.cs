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
    #region Value changed event

    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs args);

    public class ValueChangedEventArgs : EventArgs
    {
        public object OldValue { get; }
        public object NewValue { get; }
        public bool SameAsOriginal { get; }

        public ValueChangedEventArgs(object oldValue, object newValue, bool sameAsOriginal)
        {
            OldValue = oldValue;
            NewValue = newValue;
            SameAsOriginal = sameAsOriginal;
        }
    }

    #endregion Value changed event

    public abstract class SettingElement : INotifyPropertyChanged
    {
        private int _position;
        private string _label;
        private string _filter;
        private string _name;

        public object SettingParent { get; }
        public MemberInfo Member { get; }

        public string SettingName
        {
            get => _name ?? (_name = Member?.Name);
            set => _name = value;
        }

        public int Position
        {
            get => _position;
            set => Set(ref _position, value);
        }

        public string Label
        {
            get => _label;
            set => Set(ref _label, value);
        }

        public string Filter
        {
            get => _filter;
            set => Set(ref _filter, value);
        }

        public abstract int UnsavedSettings { get; }

        protected SettingElement(object settingParent, MemberInfo member)
        {
            SettingParent = settingParent;
            Member = member;
            Label = SettingName;
            var propertyChanged = settingParent as INotifyPropertyChanged;
            if (propertyChanged != null)
                propertyChanged.PropertyChanged += OuterPropertyChanged;
        }

        /// <summary>
        ///     If value is different, sets the value of a variable and calls OnPropertyChanged.
        /// </summary>
        /// <typeparam name="T">The type of the variable (infered from value).</typeparam>
        /// <param name="variable">The variable to set.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <param name="prop">The name of the property (provided by runtime).</param>
        /// <returns>True if value was changed, false otherwise.</returns>
        protected bool Set<T>(ref T variable, T value,
            [CallerMemberName] string prop = null)
        {
            if (Equals(value, variable))
                return false;
            variable = value;
            OnPropertyChanged(prop);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event ValueChangedEventHandler ValueChanged;

        protected void OnValueChanged(object oldValue, object newValue, bool sameAsOriginal)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, newValue, sameAsOriginal));
        }

        protected void OnValueChanged(object sender, ValueChangedEventArgs args)
        {
            ValueChanged?.Invoke(sender, args);
        }

        public abstract void Save();
        protected abstract void OuterPropertyChanged(object sender, PropertyChangedEventArgs e);

        /// <summary>
        ///     Checks if the filter matches the element labels.
        ///     Sets the filter field to highlight the appropriate labels.
        /// </summary>
        /// <param name="filter">The string to match to the element.</param>
        /// <param name="parentMatch">Whether the parent was matched or not.</param>
        /// <returns>True if filter matches parts of the element, false otherwise.</returns>
        public virtual bool Matches(string filter, bool parentMatch = false)
        {
            bool labelMatch = Label?.ToUpper().Contains(filter) ?? false;
            bool match = labelMatch;
            Filter = match ? filter : null;
            return parentMatch || match;
        }
    }

    public class SettingSection : SettingElement
    {
        private bool _visible;
        private ObservableCollection<SettingSection> _subSections;
        private ObservableCollection<SettingPageElement> _elements;
        private bool _isExpanded;
        private bool _isSelected;
        private BitmapSource _icon;

        public bool Visible
        {
            get => _visible;
            set => Set(ref _visible, value);
        }

        public override int UnsavedSettings
            => SubSections.Sum(s => s.UnsavedSettings) + Elements.Sum(e => e.UnsavedSettings);

        public ObservableCollection<SettingSection> SubSections
        {
            get => _subSections;
            set
            {
                if (!Set(ref _subSections, value))
                    return;
                foreach (var section in SubSections)
                    section.ValueChanged += OnValueChanged;
                SubSections.CollectionChanged += (s, e) =>
                {
                    foreach (SettingPageElement item in e.NewItems)
                        item.ValueChanged += OnValueChanged;
                };
            }
        }

        public ObservableCollection<SettingPageElement> Elements
        {
            get => _elements;
            set
            {
                if (!Set(ref _elements, value))
                    return;
                foreach (var element in Elements)
                    element.ValueChanged += OnValueChanged;
                Elements.CollectionChanged += (s, e) =>
                {
                    foreach (SettingPageElement item in e.NewItems)
                        item.ValueChanged += OnValueChanged;
                };
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public BitmapSource Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        public SettingSection(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
            Visible = true;
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

        public override bool Matches(string filter, bool parentMatch = false)
        {
            bool baseMatch = base.Matches(filter, parentMatch);
            // Using count so all elements are processed (any stops at first occurrence)
            bool childrenMatch = Elements.Count(e => e.Matches(filter, baseMatch)) > 0;
            bool subSectionsMatch = SubSections.Count(e => e.Matches(filter, baseMatch)) > 0;
            bool match = baseMatch || childrenMatch || subSectionsMatch;
            Filter = match ? filter : null;
            Visible = match;
            return match;
        }
    }

    public abstract class SettingPageElement : SettingElement, IListElement
    {
        public abstract string ResourceUri { get; }

        private string _height;
        private string _labelWidth;
        private bool _autoSave;
        private string _details;
        private string _prefix;
        private string _suffix;
        private string _suffixLabel;
        private bool _readOnly;

        public string RowHeight
        {
            get => _height;
            set => Set(ref _height, value);
        }

        public string LabelWidth
        {
            get => _labelWidth;
            set => Set(ref _labelWidth, value);
        }

        public bool AutoSave
        {
            get => _autoSave;
            set => Set(ref _autoSave, value);
        }

        public string Details
        {
            get => _details;
            set => Set(ref _details, value);
        }

        public string Prefix
        {
            get => _prefix;
            set => Set(ref _prefix, value);
        }

        public string Suffix
        {
            get => _suffix;
            set => Set(ref _suffix, value);
        }

        public string SuffixLabel
        {
            get => _suffixLabel;
            set => Set(ref _suffixLabel, value);
        }

        public bool ReadOnly
        {
            get => _readOnly;
            set => Set(ref _readOnly, value);
        }

        public bool IsReadOnly => ReadOnly || (Member?.IsReadOnly() ?? true);

        protected SettingPageElement(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
        }

        /// <summary>
        ///     Calls Set and if value was overwritten, save it.
        /// </summary>
        /// <typeparam name="T">The type of the variable (infered from value).</typeparam>
        /// <param name="variable">The variable to set.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <param name="originalValue">The variable original value, to create the ValueChanged event.</param>
        /// <param name="prop">The name of the property (provided by runtime).</param>
        /// <returns>True if value was changed, false otherwise.</returns>
        protected void SetAndSave<T>(ref T variable, T value, T originalValue,
            [CallerMemberName] string prop = null)
        {
            T oldValue = variable;
            // ReSharper disable once ExplicitCallerInfoArgument
            if (!Set(ref variable, value, prop))
                return;
            if (AutoSave)
                Save();
            OnValueChanged(oldValue, value, Equals(value, originalValue));
        }

        public override bool Matches(string filter, bool parentMatch = false)
        {
            bool baseMatch = base.Matches(filter, parentMatch);
            bool detailsMatch = Details?.ToUpper().Contains(filter) ?? false;
            bool suffixLabelMatch = SuffixLabel?.ToUpper().Contains(filter) ?? false;
            bool match = baseMatch || detailsMatch || suffixLabelMatch;
            Filter = match ? filter : null;
            return match;
        }
    }

    internal abstract class SettingGroup : SettingPageElement
    {
        private ObservableCollection<SettingPageElement> _elements;

        public ObservableCollection<SettingPageElement> Elements
        {
            get => _elements;
            set
            {
                if (!Set(ref _elements, value))
                    return;
                foreach (var element in Elements)
                    element.ValueChanged += OnValueChanged;
                Elements.CollectionChanged += (s, e) =>
                {
                    foreach (SettingPageElement item in e.NewItems)
                        item.ValueChanged += OnValueChanged;
                };
            }
        }

        public override int UnsavedSettings => Elements.Sum(e => e.UnsavedSettings);

        protected SettingGroup(object settingParent, MemberInfo member)
            : base(settingParent, member)
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

        public override bool Matches(string filter, bool parentMatch = false)
        {
            bool baseMatch = base.Matches(filter, parentMatch);
            // Using count so all elements are processed (any stops at first occurrence)
            bool childrenMatch = Elements.Count(e => e.Matches(filter, baseMatch)) > 0;
            bool match = baseMatch || childrenMatch;
            Filter = match ? filter : null;
            return match;
        }
    }

    internal class SettingGroupBox : SettingGroup
    {
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        public SettingGroupBox(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
        }
    }

    internal class SettingGroupTitle : SettingGroup
    {
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        public SettingGroupTitle(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
        }
    }

    internal class StringSetting : SettingPageElement
    {
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        private string _originalValue;
        private string _value;
        private string _placeHolderText;
        private char _separator;

        public string Value
        {
            get => _value;
            set => SetAndSave(ref _value, value, _originalValue);
        }

        public string PlaceHolderText
        {
            get => _placeHolderText;
            set => Set(ref _placeHolderText, value);
        }

        public char Separator
        {
            get => _separator;
            set => Set(ref _separator, value);
        }

        public override int UnsavedSettings => _originalValue == Value ? 0 : 1;

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private string OuterValue
        {
            get
            {
                Type type = Member.GetValueType();
                if (type == typeof(string))
                    return (string) Member.GetValue(SettingParent);
                // type == typeof(string[])
                return string.Join(Separator.ToString(), (string[]) Member.GetValue(SettingParent));
            }
            set
            {
                Type type = Member.GetValueType();
                if (type == typeof(string))
                {
                    Member.SetValue(SettingParent, value);
                }
                else // type == typeof(string[])
                {
                    string[] values = value.Split(Separator);
                    Member.SetValue(SettingParent, values);
                }
            }
        }

        public StringSetting(object parent, MemberInfo member, char separator)
            : base(parent, member)
        {
            Separator = separator;
            Value = OuterValue;
            _originalValue = Value;
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            OuterValue = Value;
            _originalValue = Value;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = OuterValue;
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
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        private NumberSettingType _type;
        private int _originalValue;
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
            get => _type;
            set => Set(ref _type, value);
        }

        public int Value
        {
            get => _value;
            set => SetAndSave(ref _value, value, _originalValue);
        }

        public int MinValue
        {
            get => _minValue;
            set => Set(ref _minValue, value);
        }

        public int MaxValue
        {
            get => _maxValue;
            set => Set(ref _maxValue, value);
        }

        public int Step
        {
            get => _step;
            set => Set(ref _step, value);
        }

        public int TickFrequency
        {
            get => _tickFrequency;
            set => Set(ref _tickFrequency, value);
        }

        public override int UnsavedSettings => _originalValue == Value ? 0 : 1;

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private int OuterValue
        {
            get => (int) Member.GetValue(SettingParent);
            set => Member.SetValue(SettingParent, value);
        }

        public NumberSetting(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
            Value = OuterValue;
            _originalValue = Value;
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            OuterValue = Value;
            _originalValue = Value;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = OuterValue;
        }
    }

    internal class TextSetting : SettingPageElement
    {
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        private string _originalValue;
        private string _value;

        public string Value
        {
            get => _value;
            set
            {
                if (_originalValue == null)
                    _originalValue = value;
                SetAndSave(ref _value, value, _originalValue);
            }
        }

        public override int UnsavedSettings => _originalValue == Value ? 0 : 1;

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private string OuterValue
        {
            get => (string) Member.GetValue(SettingParent);
            set => Member.SetValue(SettingParent, value);
        }

        public TextSetting(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
            Value = OuterValue;
            _originalValue = Value;
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            OuterValue = Value;
            _originalValue = Value;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = OuterValue;
        }
    }

    internal class BoolSetting : SettingPageElement
    {
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        private bool _originalValue;
        private bool _value;

        public bool Value
        {
            get => _value;
            set => SetAndSave(ref _value, value, _originalValue);
        }

        public override int UnsavedSettings => _originalValue == Value ? 0 : 1;

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private bool OuterValue
        {
            get => (bool) Member.GetValue(SettingParent);
            set => Member.SetValue(SettingParent, value);
        }

        public BoolSetting(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
            Value = OuterValue;
            _originalValue = Value;
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            OuterValue = Value;
            _originalValue = Value;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = OuterValue;
        }
    }

    internal class DateSetting : SettingPageElement
    {
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        private DateTime _originalValue;
        private DateTime _value;

        public DateTime Value
        {
            get => _value;
            set => SetAndSave(ref _value, value, _originalValue);
        }

        public override int UnsavedSettings => _originalValue == Value ? 0 : 1;

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private DateTime OuterValue
        {
            get => (DateTime) Member.GetValue(SettingParent);
            set => Member.SetValue(SettingParent, value);
        }

        public DateSetting(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
            Value = OuterValue;
            _originalValue = Value;
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            OuterValue = Value;
            _originalValue = Value;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                Value = OuterValue;
        }
    }

    internal class SettingField : SettingElement
    {
        public object Value { get; }
        public string Details { get; }

        public override int UnsavedSettings => 0;

        public SettingField(object value, string settingName, string label, string details)
            : base(null, null)
        {
            Value = value;
            SettingName = settingName;
            Label = label;
            Details = details;
        }

        public override string ToString()
        {
            return Label;
        }

        public override void Save()
        {
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public override bool Matches(string filter, bool parentMatch = false)
        {
            bool baseMatch = base.Matches(filter, parentMatch);
            bool detailsMatch = Details?.ToUpper().Contains(filter) ?? false;
            bool match = baseMatch || detailsMatch;
            Filter = match ? filter : null;
            return match;
        }
    }

    internal abstract class ChoiceSetting : SettingPageElement
    {
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        private ObservableCollection<SettingField> _choices;
        private SettingField _originalValue;
        private SettingField _selectedValue;

        public ObservableCollection<SettingField> Choices
        {
            get => _choices;
            set => Set(ref _choices, value);
        }

        public SettingField SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_originalValue == null)
                    _originalValue = value;
                SetAndSave(ref _selectedValue, value, _originalValue);
            }
        }

        public override int UnsavedSettings => _originalValue == _selectedValue ? 0 : 1;

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private SettingField OuterValue
        {
            get
            {
                object value = Member.GetValue(SettingParent);
                return Choices.FirstOrDefault(a => a.Value.Equals(value));
            }
            set => Member.SetValue(SettingParent, value?.Value);
        }

        protected ChoiceSetting(object settingParent, MemberInfo member, ObservableCollection<SettingField> choices)
            : base(settingParent, member)
        {
            Choices = choices;
            SelectedValue = OuterValue;
            _originalValue = SelectedValue;
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            //if (SelectedValue != null)
            OuterValue = SelectedValue;
            _originalValue = SelectedValue;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member.Name)
                SelectedValue = OuterValue;
        }

        public override bool Matches(string filter, bool parentMatch = false)
        {
            bool baseMatch = base.Matches(filter, parentMatch);
            // Using count so all elements are processed (any stops at first occurrence)
            bool childrenMatch = Choices.Count(e => e.Matches(filter, baseMatch)) > 0;
            bool match = baseMatch || childrenMatch;
            Filter = match ? filter : null;
            return match;
        }
    }

    internal class DropDownSetting : ChoiceSetting
    {
        public DropDownSetting(object settingParent, MemberInfo member, ObservableCollection<SettingField> choices)
            : base(settingParent, member, choices)
        {
        }
    }

    internal class RadioButtonsSetting : ChoiceSetting
    {
        private static int _id;
        public string GroupName { get; }
        public ICommand OnSelectionCommand { get; }

        public RadioButtonsSetting(object settingParent, MemberInfo member, ObservableCollection<SettingField> choices)
            : base(settingParent, member, choices)
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

        public ListViewSetting(object settingParent, MemberInfo member, ObservableCollection<SettingField> choices)
            : base(settingParent, member, choices)
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
        public override string ResourceUri { get; } =
            ResourceUtils.AppPath("SettingElements/SettingElementsTemplate.xaml");

        private HorizontalAlignment _alignment;
        private ButtonType _type;

        public HorizontalAlignment Alignment
        {
            get => _alignment;
            set => Set(ref _alignment, value);
        }

        public ButtonType Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        public ICommand PressedCommand { get; set; }

        public override int UnsavedSettings => 0;

        protected ButtonSetting(object settingParent, MemberInfo member)
            : base(settingParent, member)
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
        private string _originalValue;
        private string _value;

        /// <summary>
        ///     The value corresponds to the hyperlink's path.
        /// </summary>
        public string Value
        {
            get => _value;
            set => SetAndSave(ref _value, value, _originalValue);
        }

        public Action<string> SelectSection { get; set; }

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private string OuterValue
        {
            get => (string) Member?.GetValue(SettingParent);
            set => Member.SetValue(SettingParent, value);
        }

        /// <summary>
        ///     Member is null for generated links from the setting window.
        /// </summary>
        public LinkButtonSetting(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
            Value = OuterValue;
            _originalValue = Value;
        }

        protected override void OnPressed()
        {
            SelectSection?.Invoke(Value);
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            OuterValue = Value;
            _originalValue = Value;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OuterPropertyChanged(sender, e);
            if (e.PropertyName == Member.Name)
                Value = OuterValue;
        }
    }

    internal class CustomButtonSetting : ButtonSetting
    {
        private Action _originalValue;
        private Action _value;

        /// <summary>
        ///     The value corresponds to the action to execute when the button is pressed.
        /// </summary>
        public Action Value
        {
            get => _value;
            set
            {
                if (_originalValue == null)
                    _originalValue = value;
                SetAndSave(ref _value, value, _originalValue);
            }
        }

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private Action OuterValue
        {
            get => (Action) Member.GetValue(SettingParent);
            set => Member.SetValue(SettingParent, value);
        }

        public CustomButtonSetting(object settingParent, MemberInfo member)
            : base(settingParent, member)
        {
            Value = OuterValue;
            _originalValue = Value;
        }

        protected override void OnPressed()
        {
            Value();
        }

        public override void Save()
        {
            if (IsReadOnly)
                return;
            OuterValue = Value;
            _originalValue = Value;
        }

        protected override void OuterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OuterPropertyChanged(sender, e);
            if (e.PropertyName == Member.Name)
                Value = OuterValue;
        }
    }
}
