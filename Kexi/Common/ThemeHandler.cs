using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using Kexi.ViewModel;

namespace Kexi.Common
{
    [Export]
    public class ThemeHandler
    {
        private Workspace Workspace { get; }
        private NotificationHost NotificationHost { get; }
        private Options Options { get; }

        [ImportingConstructor]
        public ThemeHandler(Workspace workspace, NotificationHost notificationHost, Options options)
        {
            Workspace = workspace;
            NotificationHost = notificationHost;
            Options = options;
            InitThemes();
        }

        public List<FileInfo> Themes { get; private set; }
        public FileInfo CurrentTheme { get; private set; }

        private void InitThemes()
        {
            var themeDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Themes\"));
            Themes = themeDirectory.Exists
                ? themeDirectory.GetFiles().ToList()
                : new List<FileInfo>();
        }

        public void ChangeTheme(string themeName)
        {
            var theme = Themes.FirstOrDefault(t => t.Name.StartsWith(themeName, StringComparison.CurrentCultureIgnoreCase));
            if (theme == null)
            {
                NotificationHost.AddError($"Theme {themeName} was not found");
                return;
            }

            CurrentTheme = theme;
            var dict = new ResourceDictionary
            {
                Source = new Uri(theme.FullName, UriKind.Absolute)
            };
            Options.WriteToConfig("Theme", Path.GetFileNameWithoutExtension(theme.FullName));
            Application.Current.Resources.MergedDictionaries.Add(dict);
            NotificationHost.AddInfo("Theme changed to " + themeName);
        }

        public void MoveNext()
        {
            if (!Themes.Any())
                return;

            var index = Themes.IndexOf(CurrentTheme);
            if (++index > Themes.Count - 1)
                index = 0;
            ChangeTheme(Themes[index].Name);
        }
    }
}