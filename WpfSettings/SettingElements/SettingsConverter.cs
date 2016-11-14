using System;
using System.Collections.Generic;
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
                .OrderSettings();
            return sections;
        }

        public static ObservableCollection<SettingPageElement> GetElements(object settings, ConverterArgs e)
        {
            Type type = settings.GetType();
            MemberInfo[] members = type.GetMembers();

            // Group definitions
            var groupDefinitions = type.GetCustomAttributes<SettingGroupDefinitionAttribute>(false).ToList();
            var groups = groupDefinitions.Select(p => p.GetElement(settings, e)).OfType<SettingGroup>().ToList();

            // Elements
            var elements = new ObservableCollection<SettingPageElement>(groups);
            foreach (MemberInfo member in members)
            {
                var attribute = member.GetCustomAttribute<SettingPageAttribute>(false);
                if (attribute == null)
                    continue;
                SettingPageElement element = attribute.GetElement(settings, member, e);
                if (attribute.InGroup != null)
                    groups[groupDefinitions.FindIndex(g => g.Name == attribute.InGroup)].Elements.Add(element);
                else
                    elements.Add(element);
            }

            // Order
            foreach (SettingGroup group in groups)
                group.Elements = group.Elements.OrderSettings();
            elements = elements.OrderSettings();

            // Set positions
            foreach (SettingGroup group in groups)
                for (var i = 0; i < group.Elements.Count; i++)
                    group.Elements[i].Position = i;
            for (var i = 0; i < elements.Count; i++)
                elements[i].Position = i;
            return elements;
        }

        private static ObservableCollection<TSetting> OrderSettings<TSetting>(this IEnumerable<TSetting> elements)
            where TSetting : SettingElement
        {
            var orderedElements = elements
                .OrderBy(e => e.Position)
                .ThenBy(e => e.Member?.MetadataToken);
            return new ObservableCollection<TSetting>(orderedElements);
        }

        private static SettingSection GetSection(object parent, MemberInfo member, ConverterArgs e)
        {
            var attribute = member.GetCustomAttribute<SettingSectionAttribute>(false);
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
