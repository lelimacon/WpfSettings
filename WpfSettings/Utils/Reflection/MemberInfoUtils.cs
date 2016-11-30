using System;
using System.Reflection;

namespace WpfSettings.Utils.Reflection
{
    internal static class MemberInfoUtils
    {
        internal static Type GetValueType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).PropertyType;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static bool IsReadOnly(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).IsInitOnly;
                case MemberTypes.Property:
                    return !((PropertyInfo) memberInfo).CanWrite;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static object GetValue(this MemberInfo memberInfo, object parent)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).GetValue(parent);
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).GetValue(parent);
                default:
                    throw new NotImplementedException();
            }
        }

        internal static void SetValue(this MemberInfo memberInfo, object parent, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo) memberInfo).SetValue(parent, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo) memberInfo).SetValue(parent, value);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
