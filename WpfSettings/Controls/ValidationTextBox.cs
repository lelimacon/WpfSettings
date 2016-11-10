using System;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.Controls
{
    internal class ValidationTextBox : TextBox
    {
        private string _lastText;
        private int _lastPosition;

        public static readonly DependencyProperty ValidateInputProperty
            = MvvmUtils.RegisterDp<ValidationTextBox>();

        /// <summary>
        ///     Indicates if the new input is valid or not.
        /// </summary>
        public Func<string, bool> ValidateInput
        {
            get { return (Func<string, bool>) GetValue(ValidateInputProperty); }
            set { SetValue(ValidateInputProperty, value); }
        }

        public ValidationTextBox()
        {
            TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (!ValidText(Text))
            {
                Text = _lastText;
                CaretIndex = _lastPosition;
            }
            SaveState();
        }

        private void SaveState()
        {
            _lastText = Text;
            _lastPosition = CaretIndex;
        }

        private bool ValidText(string text)
        {
            if (ValidateInput == null)
                return true;
            return ValidateInput.Invoke(text);
        }
    }
}
