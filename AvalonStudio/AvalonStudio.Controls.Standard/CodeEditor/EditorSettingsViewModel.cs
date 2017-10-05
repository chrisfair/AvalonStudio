﻿using AvalonStudio.Controls.Standard.SettingsDialog;
using AvalonStudio.Extensibility;
using AvalonStudio.Extensibility.Editor;
using AvalonStudio.Extensibility.Plugin;
using AvalonStudio.GlobalSettings;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvalonStudio.Controls.Standard.CodeEditor
{
    public class EditorSettingsViewModel : SettingsViewModel, IExtension
    {
        private bool _removeTrailingWhiteSpaceOnSave;
        private bool _autoFormat;
        private int _selectedColorSchemeIndex;

        public EditorSettingsViewModel() : base("General")
        {
        }

        public override void OnDialogLoaded()
        {
            base.OnDialogLoaded();

            var settings = Settings.GetSettings<Controls.Standard.CodeEditor.EditorSettings>();

            SelectedColorSchemeIndex = ColorSchemes.IndexOf(settings.ColorScheme);

            RemoveTrailingWhiteSpaceOnSave = settings.RemoveTrailingWhitespaceOnSave;
            AutoFormat = settings.AutoFormat;
        }

        private void Save()
        {
            var settings = Settings.GetSettings<Controls.Standard.CodeEditor.EditorSettings>();

            settings.RemoveTrailingWhitespaceOnSave = RemoveTrailingWhiteSpaceOnSave;
            settings.AutoFormat = AutoFormat;
            settings.ColorScheme = ColorSchemes[SelectedColorSchemeIndex];

            Settings.SetSettings(settings);
        }

        public List<string> ColorSchemes => ColorScheme.ColorSchemes.Select(t => t.Name).ToList();

        public bool AutoFormat
        {
            get { return _autoFormat; }
            set
            {
                this.RaiseAndSetIfChanged(ref _autoFormat, value);

                Save();
            }
        }

        public bool RemoveTrailingWhiteSpaceOnSave
        {
            get { return _removeTrailingWhiteSpaceOnSave; }
            set
            {
                this.RaiseAndSetIfChanged(ref _removeTrailingWhiteSpaceOnSave, value);

                Save();
            }
        }

        public int SelectedColorSchemeIndex
        {
            get { return _selectedColorSchemeIndex; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedColorSchemeIndex, value);

                if (_selectedColorSchemeIndex >= 0 && ColorSchemes.Count > _selectedColorSchemeIndex)
                {
                    var loadedTheme = ColorScheme.LoadColorScheme(ColorSchemes[_selectedColorSchemeIndex]);

                    if (loadedTheme.Name != ColorSchemes[_selectedColorSchemeIndex])
                    {
                        _selectedColorSchemeIndex = ColorSchemes.IndexOf(loadedTheme.Name);
                    }

                    Save();
                }
            }
        }

        public void Activation()
        {
            IoC.Get<ISettingsManager>().RegisterSettingsDialog("Editor", this);
        }

        public void BeforeActivation()
        {

        }
    }
}
