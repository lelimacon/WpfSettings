using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfSettingsControl
{
    public class PlaceHolderTextBox : TextBox
    {
        private bool _isPlaceHolder = true;
        private string _placeHolderText;

        public string PlaceHolderText
        {
            get { return _placeHolderText; }
            set
            {
                _placeHolderText = value;
                SetPlaceHolder();
            }
        }

        public PlaceHolderTextBox()
        {
            GotFocus += RemovePlaceHolder;
            LostFocus += SetPlaceHolder;
        }

        private void SetPlaceHolder(object sender, EventArgs e)
        {
            SetPlaceHolder();
        }

        private void SetPlaceHolder()
        {
            if (!string.IsNullOrEmpty(Text))
                return;
            Text = PlaceHolderText;
            Foreground = Brushes.DarkGray;
            _isPlaceHolder = true;
        }

        private void RemovePlaceHolder(object sender, EventArgs e)
        {
            RemovePlaceHolder();
        }

        private void RemovePlaceHolder()
        {
            if (!_isPlaceHolder)
                return;
            Text = string.Empty;
            Foreground = SystemColors.WindowTextBrush;
            _isPlaceHolder = false;
        }
    }
}
