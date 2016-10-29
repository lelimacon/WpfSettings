using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.Controls
{
    public partial class NumberBox : UserControl
    {
        public static readonly DependencyProperty StepProperty
            = MvvmUtils.RegisterDp<NumberBox>();

        public static readonly DependencyProperty ValueProperty
            = MvvmUtils.RegisterDp<NumberBox>();

        /// <summary>
        ///     Indicates the delta to apply to the value.
        /// </summary>
        public int Step
        {
            get { return (int) GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the number.
        /// </summary>
        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string Text
        {
            get { return Value.ToString(); }
            set { Value = int.Parse(value); }
        }

        public Func<string, bool> ValidateInput { get; }
        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        public NumberBox()
        {
            ValidateInput = IsInputValid;
            IncrementCommand = new RelayCommand(Increment);
            DecrementCommand = new RelayCommand(Decrement);
            Step = 1;
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
            // TODO: fix binding (Value is not updated on input)
            string text = NumberTextBox.Text;
            var number = int.Parse(text);
            Value = number + Step;
        }

        private void Decrement()
        {
            string text = NumberTextBox.Text;
            var number = int.Parse(text);
            Value = number - Step;
        }

        private bool IsInputValid(string input)
        {
            int number;
            bool parsed = int.TryParse(input, out number);
            return parsed;
        }
    }
}
