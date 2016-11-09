using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
            var pageElements = new ObservableCollection<SettingPageElement>(elements);
            for (var i = 0; i < pageElements.Count; i++)
                pageElements[i].Position = i;
            return pageElements;
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

        internal static string InferLabel(string memberName)
        {
            string label = Regex.Replace(memberName.Substring(1), @"[A-Z]",
                match => " " + char.ToLower(match.Value[0]));
            string inferedLabel = char.ToUpper(memberName[0]) + label;
            return inferedLabel;
        }
    }
}
