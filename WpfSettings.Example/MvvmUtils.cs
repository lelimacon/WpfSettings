using System;
using System.ComponentModel;

namespace WpfSettings.Example
{
    internal static class MvvmUtils
    {
        public static void AddPropertyChanged(this object source, PropertyChangedEventHandler handler)
        {
            var sourceNotifier = source as INotifyPropertyChanged;
            if (sourceNotifier == null)
                throw new ArgumentException("Source must inherit INotifyPropertyChanged");
            sourceNotifier.PropertyChanged += handler;
        }

        public static void ReflectPropertyChanged(this object source, object target)
        {
            var sourceNotifier = source as INotifyPropertyChanged;
            var targetNotifier = target as INotifyPropertyChanged;
            if (sourceNotifier == null)
                throw new ArgumentException("Source must inherit INotifyPropertyChanged");
            if (targetNotifier == null)
                throw new ArgumentException("Target must inherit INotifyPropertyChanged");
            sourceNotifier.PropertyChanged += (s, e) => { ((dynamic) target).OnPropertyChanged(e.PropertyName); };
        }

        public delegate string Matcher(string propertyName);

        public static void ReflectPropertyChanged(this object source, object target, Matcher matcher)
        {
            var sourceNotifier = source as INotifyPropertyChanged;
            var targetNotifier = target as INotifyPropertyChanged;
            if (sourceNotifier == null)
                throw new ArgumentException("Source must inherit INotifyPropertyChanged");
            if (targetNotifier == null)
                throw new ArgumentException("Target must inherit INotifyPropertyChanged");
            if (matcher == null)
                throw new ArgumentNullException(nameof(matcher));
            sourceNotifier.PropertyChanged += (s, e) =>
            {
                string propertyName = matcher(e.PropertyName);
                if (!string.IsNullOrEmpty(propertyName))
                    ((dynamic) target).OnPropertyChanged(propertyName);
            };
        }
    }
}
