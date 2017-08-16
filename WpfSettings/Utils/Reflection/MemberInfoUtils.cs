using System;
using System.Reflection;

namespace WpfSettings.Utils.Reflection
{
    public static class MemberInfoUtils
    {
        public static Type GetValueType(this MemberInfo memberInfo)
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

        public static bool IsReadOnly(this MemberInfo memberInfo)
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

        public static object GetValue(this MemberInfo memberInfo, object parent)
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

        public static void SetValue(this MemberInfo memberInfo, object parent, object value)
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
