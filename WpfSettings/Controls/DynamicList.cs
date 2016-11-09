using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.Utils.Wpf;

namespace WpfSettings.Controls
{
    internal interface IListElement
    {
        int Position { get; }
        string Height { get; }
    }

    internal class DynamicList : Grid
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            MvvmUtils.RegisterDp<DynamicList>(ItemsChanged);

        public IEnumerable<IListElement> ItemsSource
        {
            get { return (IEnumerable<IListElement>) GetValue(ItemsSourceProperty); }
            set { SetValueDp(ItemsSourceProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        private void SetRowDefinitions()
        {
            foreach (IListElement item in ItemsSource)
            {
                GridLength height = GetHeight(item.Height);
                RowDefinition row = new RowDefinition {Height = height};
                RowDefinitions.Add(row);
            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static GridLength GetHeight(string height)
        {
            return string.IsNullOrEmpty(height)
                ? GridLength.Auto
                : (GridLength) new GridLengthConverter().ConvertFromString(height);
        }

        private static void ItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DynamicList dynamicList = (DynamicList) d;
            dynamicList.SetRowDefinitions();
        }
    }
}
