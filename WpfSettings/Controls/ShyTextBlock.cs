using System.Windows;
using System.Windows.Controls;

namespace WpfSettings.Controls
{
    internal class ShyTextBlock : TextBlock
    {
        public ShyTextBlock()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (string.IsNullOrEmpty(Text))
                Visibility = Visibility.Collapsed;
        }
    }
}
