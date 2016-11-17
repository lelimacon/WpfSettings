using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

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

        internal static DependencyProperty RegisterDp<T>(PropertyMetadata metadata,
            [CallerMemberName] string prop = null)
        {
            return RegisterDp(typeof(T), prop, metadata);
        }

        private static DependencyProperty RegisterDp(Type parentType, string prop, object defaultValue,
            PropertyChangedCallback propertyChanged)
        {
            PropertyMetadata metadata = GetMetadata(defaultValue, propertyChanged);
            return RegisterDp(parentType, prop, metadata);
        }

        private static DependencyProperty RegisterDp(Type parentType, string prop, PropertyMetadata metadata)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));
            string ending = "Property";
            if (!prop.EndsWith(ending))
                throw new ArgumentException($"Property name must end with {ending}: {prop}");
            prop = prop.Remove(prop.Length - ending.Length);
            PropertyInfo propertyInfo = parentType.GetProperty(prop);
            Type type = propertyInfo.PropertyType;
            return DependencyProperty.Register(prop, type, parentType, metadata);
        }

        private static PropertyMetadata GetMetadata(object defaultValue,
            PropertyChangedCallback propertyChanged)
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                PropertyChangedCallback = propertyChanged,
            };
            if (defaultValue != null)
                metadata.DefaultValue = defaultValue;
            return metadata;
        }
    }
}
