﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.Controls
{
    public partial class ExtendedTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty
            = MvvmUtils.RegisterDp<ExtendedTextBox>();

        public static readonly DependencyProperty PrefixProperty
            = MvvmUtils.RegisterDp<ExtendedTextBox>();

        public static readonly DependencyProperty SuffixProperty
            = MvvmUtils.RegisterDp<ExtendedTextBox>();

        /// <summary>
        ///     Gets or sets the text content.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValueDp(TextProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the textbox prefix.
        /// </summary>
        public string Prefix
        {
            get { return (string) GetValue(PrefixProperty); }
            set { SetValueDp(PrefixProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the textbox suffix.
        /// </summary>
        public string Suffix
        {
            get { return (string) GetValue(SuffixProperty); }
            set { SetValueDp(SuffixProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public ExtendedTextBox()
        {
            InitializeComponent();
        }
    }
}
