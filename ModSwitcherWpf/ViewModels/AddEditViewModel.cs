﻿using System;
using System.Windows.Input;
using ModSwitcherLib;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ModSwitcherWpf.ViewModels
{

    public class AddEditViewModel : ViewModelBase
    {
        #region Constructors
        public AddEditViewModel()
        {
            TheMod = new Mod();
        }

        public AddEditViewModel(string windowName, string selectedModName
                                , List<string> versionNames, Action closeAction)
        {
            WindowName = windowName;
            SelectedModName = selectedModName;
            VersionNames = versionNames;
            CloseAction = closeAction;
            switch (windowName)
            {
                case "Add Mod":
                    TheMod = new Mod();
                    break;

                case "Edit Mod":
                    try
                    {
                        TheMod = XMLConfig.ReadMod(selectedModName);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show($"Failed to load mod: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CloseAction?.Invoke();
                    }
                    break;
            }
        }
        #endregion

        #region Properties
        public string WindowName { get; set; }

        public Mod TheMod { get; set; }

        public string TheModName
        {
            get
            {
                return TheMod.ModName;
            }
            set
            {
                TheMod.ModName = value;
                OnPropertyChanged("OKEnabled");
            }
        }

        public string SelectedModName { get; set; }

        public bool OverrideGamePath
        {
            get
            {
                return TheMod.OverrideGamePath;
            }
            set
            {
                TheMod.OverrideGamePath = value;
                OnPropertyChanged("OverrideGamePath");
            }
        }

        public bool SetVersion
        {
            get
            {
                return TheMod.SetVersion;
            }
            set
            {
                TheMod.SetVersion = value;
                OnPropertyChanged("SetVersion");
            }
        }

        public bool OKEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TheMod.ModName);
            }
        }

        public List<string> VersionNames { get; set; }

        public Action CloseAction, RefreshMainResourcesAction;
        #endregion

        #region Commands
        private void OK()
        {
            switch (WindowName)
            {
                case "Add Mod":

                    try
                    {
                        if (!XMLConfig.Exists(TheMod.ModName))
                        {
                            XMLConfig.AddMod(TheMod);
                            RefreshMainResourcesAction?.Invoke();
                            CloseAction?.Invoke();
                        }
                        else
                        {
                            MessageBox.Show($"There already exists a mod called {TheMod.ModName}!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show($"Failed to add mod: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case "Edit Mod":

                    try
                    {
                        int indexOfSelectedMod = XMLConfig.IndexOf(SelectedModName);

                        if (!XMLConfig.Exists(TheMod.ModName, indexOfSelectedMod))
                        {
                            if (XMLConfig.ReadCurrentModName() == SelectedModName)
                            {
                                XMLConfig.SetCurrentModName(TheMod.ModName);
                            }
                            XMLConfig.EditMod(TheMod, SelectedModName);

                            RefreshMainResourcesAction?.Invoke();
                            CloseAction?.Invoke();
                        }
                        else
                        {
                            MessageBox.Show($"There already exists a mod called {TheMod.ModName}!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show($"Failed to edit mod: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

            }
        }

        private void OpenFileFolderDialog()
        {
            switch (TheMod.modType)
            {
                case ModType.File:

                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = "Select the Mod Path";
                    openFileDialog.Filter = "BIG files (*.big)|*.big|All files (*.*)|*.*";
                    var result = openFileDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        TheMod.ModPath = openFileDialog.FileName;
                        OnPropertyChanged("TheMod");
                    }
                    break;

                case ModType.Folder:

                    FolderBrowserDialog folderBrowseDialog = new FolderBrowserDialog();
                    result = folderBrowseDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        TheMod.ModPath = folderBrowseDialog.SelectedPath;
                        OnPropertyChanged("TheMod");
                    }
                    break;
            }

        }

        private void OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select the Game Path";
            openFileDialog.Filter = "EXE files (*.exe)|*.exe|All files (*.*)|*.*";
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                TheMod.GamePath = openFileDialog.FileName;
                OnPropertyChanged("TheMod");
            }
        }

        private void Cancel()
        {
            CloseAction?.Invoke();
        }

        public ICommand OKCommand
        {
            get
            {
                return new DelegateCommand(OK);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new DelegateCommand(Cancel);
            }
        }

        public ICommand OpenFileFolderDialogCommand
        {
            get
            {
                return new DelegateCommand(OpenFileFolderDialog);
            }
        }

        public ICommand OpenFileDialogCommand
        {
            get
            {
                return new DelegateCommand(OpenFileDialog);
            }
        }
        #endregion
    }
}
