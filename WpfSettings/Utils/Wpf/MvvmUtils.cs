using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfSettings.Utils.Wpf
{
    internal static class MvvmUtils
    {
        internal static DependencyProperty RegisterDp<T>([CallerMemberName] string prop = null)
        {
            return RegisterDp(typeof(T), prop, null, null);
        }

        internal static DependencyProperty RegisterDp<T>(object defaultValue,
            [CallerMemberName] string prop = null)
        {
            return RegisterDp(typeof(T), prop, defaultValue, null);
        }

        internal static DependencyProperty RegisterDp<T>(PropertyChangedCallback propertyChanged,
            [CallerMemberName] string prop = null)
        {
            return RegisterDp(typeof(T), prop, null, propertyChanged);
        }

        internal static DependencyProperty RegisterDp<T>(object defaultValue,
            PropertyChangedCallback propertyChanged,
            [CallerMemberName] string prop = null)
        {
            return RegisterDp(typeof(T), prop, defaultValue, propertyChanged);
        }

        private static DependencyProperty RegisterDp(Type parentType, string prop, object defaultValue,
            PropertyChangedCallback propertyChanged)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));
            string ending = "Property";
            if (!prop.EndsWith(ending))
                throw new ArgumentException($"Property name must end with {ending}: {prop}");
            prop = prop.Remove(prop.Length - ending.Length);
            PropertyInfo propertyInfo = parentType.GetProperty(prop);
            Type type = propertyInfo.PropertyType;
            PropertyMetadata metadata = GetMetadata(defaultValue, propertyChanged);
            return DependencyProperty.Register(prop, type, parentType, metadata);
        }

        private static PropertyMetadata GetMetadata(object defaultValue,
            PropertyChangedCallback propertyChanged)
        {
            if (defaultValue == null)
                return new FrameworkPropertyMetadata(propertyChanged);
            return new FrameworkPropertyMetadata(defaultValue, propertyChanged);
        }
    }
}
