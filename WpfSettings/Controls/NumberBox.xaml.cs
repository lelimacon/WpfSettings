using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.Controls
{
    public partial class NumberBox : UserControl
    {
        public static readonly DependencyProperty ValueProperty
            = MvvmUtils.RegisterDp<NumberBox>(0);

        public static readonly DependencyProperty StepProperty
            = MvvmUtils.RegisterDp<NumberBox>(1);

        public static readonly DependencyProperty MinValueProperty
            = MvvmUtils.RegisterDp<NumberBox>(int.MinValue);

        public static readonly DependencyProperty MaxValueProperty
            = MvvmUtils.RegisterDp<NumberBox>(int.MaxValue);

        /// <summary>
        ///     Gets or sets the value of the number.
        /// </summary>
        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValueDp(ValueProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the delta to apply to the value.
        ///     Defaults to 1.
        /// </summary>
        public int Step
        {
            get { return (int) GetValue(StepProperty); }
            set { SetValueDp(StepProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the minimum value (inclusive) of the number.
        ///     Defaults to min int.
        /// </summary>
        public int MinValue
        {
            get { return (int) GetValue(MinValueProperty); }
            set { SetValueDp(MinValueProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the maximum value (inclusive) of the number.
        ///     Defaults to max int.
        /// </summary>
        public int MaxValue
        {
            get { return (int) GetValue(MaxValueProperty); }
            set { SetValueDp(MaxValueProperty, value); }
        }

        public Func<string, bool> ValidateInput { get; }
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public NumberBox()
        {
            ValidateInput = IsInputValid;
            IncrementCommand = new RelayCommand(Increment);
            DecrementCommand = new RelayCommand(Decrement);
            InitializeComponent();
            PreviewKeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                Increment();
            else if (e.Key == Key.Down)
                Decrement();
        }

        private void Increment()
        {
            if (string.IsNullOrEmpty(NumberTextBox.Text))
                Value = 0;
            // TODO: fix binding (Value is not updated on input)
            string text = NumberTextBox.Text;
            var number = int.Parse(text);
            Value = number + Step;
        }

        private void Decrement()
        {
            if (string.IsNullOrEmpty(NumberTextBox.Text))
                Value = 0;
            string text = NumberTextBox.Text;
            var number = int.Parse(text);
            Value = number - Step;
        }

        private bool IsInputValid(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
            int value;
            bool parsed = int.TryParse(input, out value);
            return parsed && value >= MinValue && value <= MaxValue;
        }
    }
}
