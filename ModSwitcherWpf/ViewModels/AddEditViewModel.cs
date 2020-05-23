using System;
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

        public AddEditViewModel(string windowName, string selectedModName, Action closeAction)
        {
            WindowName = windowName;
            SelectedModName = selectedModName;
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

        public bool OverrideGameFolder
        {
            get
            {
                return TheMod.OverrideGameFolder;
            }
            set
            {
                TheMod.OverrideGameFolder = value;
                OnPropertyChanged("OverrideGameFolder");
            }
        }

        public bool OKEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TheMod.ModName);
            }
        }

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

                    var openFileDialog = new OpenFileDialog();
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

                    var folderBrowseDialog = new FolderBrowserDialog();
                    result = folderBrowseDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        TheMod.ModPath = folderBrowseDialog.SelectedPath;
                        OnPropertyChanged("TheMod");
                    }
                    break;
            }

        }

        private void OpenGameFolderDialog()
        {
            var folderBrowseDialog = new FolderBrowserDialog();
            folderBrowseDialog.Description = "Select the Game Folder:";
            var result = folderBrowseDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                TheMod.GameFolder = folderBrowseDialog.SelectedPath;
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

        public ICommand OpenGameFolderDialogCommand
        {
            get
            {
                return new DelegateCommand(OpenGameFolderDialog);
            }
        }
        #endregion
    }
}
