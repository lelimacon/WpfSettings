using System;
using System.Reflection;
using System.Text.RegularExpressions;
using WpfSettings.SettingElements;
using WpfSettings.Utils.Reflection;

namespace WpfSettings.Example.CustomSettings
{
    /// <summary>
    ///     A random setting that gives random numbers.
    /// </summary>
    public class RandomSettingAttribute : SettingPageAttribute
    {
        public override SettingPageElement GetElement(object parent, MemberInfo member, ConverterArgs e)
        {
            Type type = member.GetValueType();
            if (type != typeof(int))
                throw new ArgumentException("RandomStringAttribute must target an integer");
            e = new ConverterArgs(e, this);
            var element = new RandomSetting(parent, member)
            {
                ReadOnly = e.IsReadOnly == ReadOnly.Yes || e.IsReadOnly == ReadOnly.YesRecursive,
                Label = Label ?? InferLabel(member.Name),
                LabelWidth = e.LabelWidth,
                Position = Position,
                AutoSave = e.AutoSave
            };
            if (Details != null)
                element.Details = Details;
            return element;
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
