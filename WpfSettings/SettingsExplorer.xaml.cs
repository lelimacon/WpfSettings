using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfSettings.SettingElements;
using WpfSettings.Utils.Wpf;

namespace WpfSettings
{
    internal partial class SettingsExplorer : UserControl
    {
        public static readonly DependencyProperty ItemsProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>(ItemsChanged);

        public static readonly DependencyProperty FilterProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>(FilterChanged);

        public static readonly DependencyProperty ChangeActionProperty =
            MvvmUtils.RegisterDp<SettingsExplorer>(new FrameworkPropertyMetadata());

        public ObservableCollection<SettingSection> Items
        {
            get { return (ObservableCollection<SettingSection>) GetValue(ItemsProperty); }
            set { SetValueDp(ItemsProperty, value); }
        }

        public string Filter
        {
            get { return (string) GetValue(FilterProperty); }
            set { SetValueDp(FilterProperty, value); }
        }

        public Action<SettingSection> ChangeAction
        {
            get { return (Action<SettingSection>) GetValue(ChangeActionProperty); }
            set { SetValueDp(ChangeActionProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value,
            [CallerMemberName] string p = null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public SettingsExplorer()
        {
            InitializeComponent();
            ItemsTreeView.Focus();
        }

        private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SettingSection section = (SettingSection) e.NewValue;
            ChangeAction?.Invoke(section);
        }

        private static void ItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsExplorer explorer = (SettingsExplorer) d;
            if (explorer.Items.Any())
                explorer.SelectSection(0);
        }

        public void SelectSection(int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new IndexOutOfRangeException();
            SettingSection section = SearchSection(Items, index);
            if (section != null)
                section.IsSelected = true;
            ItemsTreeView.Focus();
        }

        public void SelectSection(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path must not be empty");
            string[] names = path.Split('.');
            SettingSection section = Items.FirstOrDefault(s => s.Name == names[0]);
            for (int i = 1; i < names.Length; i++)
                section = section?.SubSections?.FirstOrDefault(s => s.Name == names[i]);
            if (section == null)
                throw new ArgumentException("Invalid path");
            section.IsSelected = true;
            ItemsTreeView.Focus();
        }

        private SettingSection SearchSection(IEnumerable<SettingSection> sections, int index)
        {
            foreach (SettingSection s in sections)
            {
                SettingSection childSection = SearchSection(s, ref index);
                if (childSection != null)
                    return childSection;
            }
            return null;
        }

        private SettingSection SearchSection(SettingSection section, ref int index)
        {
            if (index == 0)
                return section;
            index--;
            if (section.SubSections == null)
                return null;
            foreach (SettingSection s in section.SubSections)
            {
                SettingSection childSection = SearchSection(s, ref index);
                if (childSection != null)
                    return childSection;
            }
            return null;
        }

        private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsExplorer explorer = (SettingsExplorer) d;
            string filter = (e.NewValue as string)?.ToUpper();
            foreach (SettingSection section in explorer.Items)
                section.Matches(filter);
        }
    }
}
