using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace WpfSettings.SettingElements
{
    internal static class SettingsConverter
    {
        public static ObservableCollection<SettingSection> GetSections(object settings, ConverterArgs e)
        {
            MemberInfo[] members = settings.GetType().GetMembers();
            var sections = members
                .Select(p => GetSection(settings, p, e))
                .Where(s => s != null)
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Member.MetadataToken);
            return new ObservableCollection<SettingSection>(sections);
        }

        public static ObservableCollection<SettingPageElement> GetElements(object settings, ConverterArgs e)
        {
            MemberInfo[] members = settings.GetType().GetMembers();
            var elements = members
                .Select(p => GetElement(settings, p, e))
                .Where(s => s != null)
                .OrderBy(s => s.Position)
                .ThenBy(s => s.Member.MetadataToken);
            return new ObservableCollection<SettingPageElement>(elements);
        }

        private static SettingSection GetSection(object parent, MemberInfo member, ConverterArgs e)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
            return attribute?.GetElement(parent, member, e);
        }

        private static SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            var attribute = member.GetCustomAttribute<SettingPageAttribute>(false);
            return attribute?.GetElement(parent, member, e);
        }
    }
}
