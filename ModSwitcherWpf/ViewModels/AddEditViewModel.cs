using System;
using System.Windows.Input;
using ModSwitcherLib;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using ModSwitcherLib.Types;

namespace ModSwitcherWpf.ViewModels
{
    public class AddEditViewModel : ViewModelBase
    {
        public AddEditViewModel()
        {
            TheMod = new Mod();
        }

        public AddEditViewModel(string windowName, MainViewModel parent)
        {
            WindowName = windowName;
            Parent = parent;
            switch (windowName)
            {
                case "Add Mod":
                    TheMod = new Mod();
                    break;

                case "Edit Mod":
                    TheMod = new Mod(parent.SelectedMod);
                    break;
            }
        }

        public string WindowName { get; set; }

        public MainViewModel Parent { get; set; }

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

        public Mod SelectedMod
        {
            get
            {
                return Parent.SelectedMod;
            }
        }

        public int IndexOfSelectedMod 
        {
            get
            {
                return Parent.ModList.IndexOf(Parent.SelectedMod);
            }
        }

        public ObservableCollection<Mod> ModList
        {
            get
            {
                return Parent.ModList;
            }
            set
            {
                Parent.ModList = value;
            }
        }

        public Mod CurrentMod 
        {
            get
            {
                return Parent.CurrentMod;
            }
            set
            {
                Parent.CurrentMod = value;
            }
        }

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

        public bool OKEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(TheMod.ModName);
            }
        }

        public Action CloseEvent;

        private void OK()
        {
            switch (WindowName)
            {
                case "Add Mod":
                
                    if (!ModList.Where(mod => mod.ModName == TheMod.ModName).Any())
                    {
                        ModList.Add(TheMod);
                        XMLConfig.WriteXML(ModList, CurrentMod);
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

                    if(!ModList.Where(mod => mod.ModName == TheMod.ModName && ModList.IndexOf(mod) != IndexOfSelectedMod).Any())
                    {
                        if(CurrentMod == SelectedMod)
                        {
                            CurrentMod = TheMod;
                        }
                        ModList.Insert(IndexOfSelectedMod, TheMod);
                        ModList.RemoveAt(IndexOfSelectedMod);
                        XMLConfig.WriteXML(ModList, CurrentMod);
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

        private void OpenFileDialog()
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            var result = openFiledialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                TheMod.GamePath = openFiledialog.FileName;
                OnPropertyChanged("TheMod");
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

        public ICommand OpenFileDialogCommand
        {
            get
            {
                return new DelegateCommand(OpenFileDialog);
            }
        }

        public ICommand OpenFileFolderDialogCommand
        {
            get
            {
                return new DelegateCommand(OpenFileFolderDialog);
            }
        }
    }
}
