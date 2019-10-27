using System;
using System.Windows.Input;
using ModSwitcherLib;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using ModSwitcherLib.Types;
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

        public AddEditViewModel(string windowName, string selectedModName, List<string> versionNames)
        {
            WindowName = windowName;
            SelectedModName = selectedModName;
            VersionNames = versionNames;
            switch (windowName)
            {
                case "Add Mod":
                    TheMod = new Mod();
                    break;

                case "Edit Mod":
                    TheMod = XMLConfig.ReadMod(selectedModName);
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

        public bool UsingModPath
        {
            get
            {
                return TheMod.UsingModPath;
            }
            set
            {
                TheMod.UsingModPath = value;
                OnPropertyChanged("UsingModPath");
                OnPropertyChanged("UsingFlag");
            }
        }

        public bool UsingFlag
        {
            get
            {
                return !(TheMod.UsingModPath);
            }
            set
            {
                TheMod.UsingModPath = !value;
                OnPropertyChanged("UsingModPath");
                OnPropertyChanged("UsingFlag");
            }
        }

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

        public bool SetRotWKVersion
        {
            get
            {
                return TheMod.SetRotWKVersion;
            }
            set
            {
                TheMod.SetRotWKVersion = value;
                OnPropertyChanged("SetRotWKVersion");
            }
        }

        public bool OKEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(TheMod.ModName);
            }
        }

        public List<string> VersionNames { get; set; }

        public Action CloseEvent, RefreshMainResourcesAction;
        #endregion

        #region Commands
        private void OK()
        {
            switch (WindowName)
            {
                case "Add Mod":

                    if (!XMLConfig.Exists(TheMod.ModName))
                    {
                        XMLConfig.AddMod(TheMod);
                        if (RefreshMainResourcesAction != null)
                        {
                            RefreshMainResourcesAction();
                        }
                        if (CloseEvent != null)
                        {
                            CloseEvent();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"There already exists a mod called {TheMod.ModName}!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case "Edit Mod":

                    int indexOfSelectedMod = XMLConfig.IndexOf(SelectedModName);

                    if (!XMLConfig.Exists(TheMod.ModName, indexOfSelectedMod))
                    {
                        if(XMLConfig.ReadCurrentModName() == SelectedModName)
                        {
                            XMLConfig.SetCurrentModName(TheMod.ModName);
                        }
                        XMLConfig.EditMod(TheMod, SelectedModName);

                        if (RefreshMainResourcesAction != null)
                        {
                            RefreshMainResourcesAction();
                        }
                        if (CloseEvent != null)
                        {
                            CloseEvent();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"There already exists a mod called {TheMod.ModName}!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                TheMod.GamePath = openFileDialog.FileName;
                OnPropertyChanged("TheMod");
            }
        }

        private void Cancel()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
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
