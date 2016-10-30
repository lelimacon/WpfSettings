using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfSettings.Utils.Wpf
{
    internal static class MvvmUtils
    {
        /*
        private static readonly Dictionary<Type, Dictionary<string, DependencyProperty>> _depProps;

        static MvvmUtils()
        {
            _depProps = new Dictionary<Type, Dictionary<string, DependencyProperty>>();
        }

        internal static DependencyProperty Dp(object parent,
            [CallerMemberName] string prop = null)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));
            Type parentType = parent.GetType();
            return Dp(parentType, prop);
        }

        internal static DependencyProperty Dp<T>([CallerMemberName] string prop = null)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));
            return Dp(typeof(T), prop);
        }

        private static DependencyProperty Dp(Type parent, string prop)
        {
            if (!_depProps.ContainsKey(parent))
                _depProps[parent] = new Dictionary<string, DependencyProperty>();
            if (!_depProps[parent].ContainsKey(prop))
                _depProps[parent][prop] = RegisterDp(parent, prop);
            return _depProps[parent][prop];
        }
        */

        internal static DependencyProperty RegisterDp(object parent, object defaultValue = null,
            [CallerMemberName] string prop = null)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));
            Type parentType = parent.GetType();
            string ending = "Property";
            if (!prop.EndsWith(ending))
                throw new ArgumentException($"Property name must end with {ending}: {prop}");
            prop = prop.Remove(prop.Length - ending.Length);
            return RegisterDp(parentType, prop, defaultValue);
        }

        internal static DependencyProperty RegisterDp<T>(object defaultValue = null,
            [CallerMemberName] string prop = null)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));
            const string ending = "Property";
            if (!prop.EndsWith(ending))
                throw new ArgumentException($"Property name must end with {ending}: {prop}");
            prop = prop.Remove(prop.Length - ending.Length);
            return RegisterDp(typeof(T), prop, defaultValue);
        }

        private static DependencyProperty RegisterDp(Type parent, string prop, object defaultValue = null)
        {
            PropertyInfo propertyInfo = parent.GetProperty(prop);
            Type type = propertyInfo.PropertyType;
            PropertyMetadata metadata = defaultValue == null ? null : new PropertyMetadata(defaultValue);
            return DependencyProperty.Register(prop, type, parent, metadata);
        }
    }
}
