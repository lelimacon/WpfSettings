using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using WpfSettings.SettingElements;
using WpfSettings.Utils.Reflection;

namespace WpfSettings.Example.CustomSettings
{
    /// <summary>
    ///     A random setting that gives random numbers.
    /// </summary>
    internal class RandomSetting : SettingPageElement
    {
        public override string ResourceUri { get; } =
            "pack://application:,,,/WpfSettings.Example;component/CustomSettings/RandomSetting.xaml";

        private readonly Random _generator;
        private int _originalValue;
        private int _value;

        public int Value
        {
            get => _value;
            set => SetAndSave(ref _value, value, _originalValue);
        }

        /// <summary>
        ///     The value of the source setting property.
        /// </summary>
        private int OuterValue
        {
            get => (int) Member.GetValue(SettingParent);
            set => Member.SetValue(SettingParent, value);
        }

        public ICommand GenRandomCommand { get; set; }

        public override int UnsavedSettings => _originalValue == Value ? 0 : 1;

        public RandomSetting(object parent, MemberInfo member)
            : base(parent, member)
        {
            _generator = new Random();
            Value = OuterValue;
            _originalValue = Value;
            GenRandomCommand = new RelayCommand(GenRandom);
        }

        private void GenRandom()
        {
            Value = _generator.Next();
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
}
