using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfSettings.Utils.Wpf
{
    internal class ValidationTextBox : TextBox
    {
        public static readonly DependencyProperty ValidateInputProperty
            = MvvmUtils.RegisterDp<ValidationTextBox>();

        /// <summary>
        ///     Indicates if the input is valid or not.
        /// </summary>
        public Func<string, bool> ValidateInput
        {
            get { return (Func<string, bool>) GetValue(ValidateInputProperty); }
            set { SetValue(ValidateInputProperty, value); }
        }

        public ValidationTextBox()
        {
            PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(this, Handler);
        }

        private void Handler(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string text = e.DataObject.GetData(typeof(string)) as string;
            if (!ValidText(text))
                e.CancelCommand();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidText(e.Text);
        }

        private bool ValidText(string text)
        {
            return ValidateInput?.Invoke(text) == true;
        }
    }
}
