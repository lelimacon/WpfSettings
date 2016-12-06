using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.Controls
{
    /// <summary>
    ///     TextBlock that supports automatic highlighting parts of the text.
    /// </summary>
    public partial class ExtendedTextBlock : UserControl
    {
        public static readonly DependencyProperty TextProperty
            = MvvmUtils.RegisterDp<ExtendedTextBlock>(OnUpdate);

        public static readonly DependencyProperty HighlightTextProperty
            = MvvmUtils.RegisterDp<ExtendedTextBlock>(OnUpdate);

        public static readonly DependencyProperty HighlightColorProperty
            = MvvmUtils.RegisterDp<ExtendedTextBlock>(Brushes.Yellow, OnUpdate);

        /// <summary>
        ///     Gets or sets the box content.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValueDp(TextProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the highlighted text.
        /// </summary>
        public string HighlightText
        {
            get { return (string) GetValue(HighlightTextProperty); }
            set { SetValueDp(HighlightTextProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the highlight color.
        ///     Defaults to Yellow.
        /// </summary>
        public SolidColorBrush HighlightColor
        {
            get { return (SolidColorBrush) GetValue(HighlightColorProperty); }
            set { SetValueDp(HighlightColorProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public ExtendedTextBlock()
        {
            InitializeComponent();
        }

        private void Update()
        {
            Block.Inlines.Clear();
            if (string.IsNullOrEmpty(HighlightText))
                Block.Text = Text;
            else
                Block.Inlines.AddRange(GetRuns());
        }

        private IEnumerable<Run> GetRuns()
        {
            Brush background = HighlightColor;
            char[] array = Text.ToCharArray();
            int li = 0;
            int i;
            while ((i = Text.IndexOf(HighlightText, li, StringComparison.InvariantCultureIgnoreCase)) >= 0)
            {
                if (i > li)
                {
                    yield return new Run(new string(array, li, i - li));
                }
                var run = new Run
                {
                    Text = new string(array, i, HighlightText.Length),
                    Background = background
                };
                yield return run;
                li = i + HighlightText.Length;
            }
            if (li < array.Length)
                yield return new Run(new string(array, li, array.Length - li));
        }

        private static void OnUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedTextBlock @this = (ExtendedTextBlock) d;
            @this.Update();
        }
    }
}
