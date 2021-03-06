﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Kexi.ViewModel;

namespace Kexi.Common
{
    public class ThemeHandler
    {
        public ThemeHandler(Workspace workspace)
        {
            Workspace = workspace;
            InitThemes();
        }

        private Workspace Workspace { get; }

        public List<FileInfo> Themes       { get; private set; }
        public FileInfo       CurrentTheme { get; private set; }

        public string CurrentThemeName => Path.GetFileNameWithoutExtension(CurrentTheme.FullName);

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
                Workspace.NotificationHost.AddError($"Theme {themeName} was not found");
                return;
            }

            CurrentTheme = theme;
            var dict = new ResourceDictionary
            {
                Source = new Uri(theme.FullName, UriKind.Absolute)
            };
            var th = Path.GetFileNameWithoutExtension(theme.FullName);
            Workspace.Options.WriteToConfig("Theme", th);
            Application.Current.Resources.MergedDictionaries.Add(dict);
            Workspace.Options.Theme = th;
            Workspace.NotificationHost.AddInfo("Theme changed to " + themeName);
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