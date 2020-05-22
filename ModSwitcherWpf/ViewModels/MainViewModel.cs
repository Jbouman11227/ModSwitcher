using System;
using ModSwitcherLib;
using ModSwitcherWpf.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace ModSwitcherWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructors
        public MainViewModel()
        {

        }
        
        public MainViewModel(Action closeAction)
        {
            CurrentModName = null;
            ModNameList = new ObservableCollection<string>();
            SelectedModName = null;
            CloseAction = closeAction;

            try
            {
                if (!File.Exists("config.xml"))
                {
                    XMLConfig.WriteNewConfig();
                }
                string currentModName = null;
                XMLConfig.ReadXML(ModNameList, ref currentModName);
                CurrentModName = currentModName;
                if (string.IsNullOrEmpty(XMLConfig.ReadGamePath()))
                {
                    bool clickedOK = (new GamePathWindow()).ShowDialogResult();
                    if (!clickedOK && CloseAction != null)
                    {
                        CloseAction();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to create or load the configuration: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseAction?.Invoke();
                return;
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<string> ModNameList { get; set; }

        private string _currentModName;

        public string CurrentModName
        {
            get
            {
                return _currentModName;
            }
            set
            {
                _currentModName = value;
                OnPropertyChanged("CurrentModName");
                OnPropertyChanged("StartGameEnabled");
            }
        }

        private string _selectedModName;
        public string SelectedModName
        {
            get
            {
                return _selectedModName;
            }
            set
            {
                _selectedModName = value;
                OnPropertyChanged("RemoveSetasCurrentEnabled");
            }
        }

        public bool StartGameEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(CurrentModName);
            }
        }

        public bool RemoveSetasCurrentEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedModName);
            }
        }

        public Action CloseAction;
        #endregion

        #region Commands
        private void StartGame()
        {
            try 
            {
                var currentMod = XMLConfig.ReadMod(CurrentModName);
                var gameFilePath = GetGamePath(currentMod) + XMLConfig.ReadGameFile(); 

                string flag = string.Empty;
                if (!string.IsNullOrWhiteSpace(currentMod.ModPath))
                {
                    flag += $"-mod \"{currentMod.ModPath}\"";
                }
                if (!string.IsNullOrWhiteSpace(currentMod.ExtraFlags))
                {
                    flag += (flag == string.Empty ? string.Empty : " ") + currentMod.ExtraFlags.Trim();
                }
         
                Process.Start($"\"{gameFilePath}\"", flag);
                CloseAction?.Invoke();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to start the current mod: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Add()
        {
            var addEditWindow = new AddEditWindow("Add Mod", SelectedModName);
            addEditWindow.addEditViewModel.RefreshMainResourcesAction = new Action(RefreshMainResources);
            addEditWindow.ShowDialog();
        }

        private void SetAsCurrent()
        {
            CurrentModName = SelectedModName;
            try
            {
                XMLConfig.SetCurrentModName(SelectedModName);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Failed to set current mod: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PatchSwitcher()
        {
            try
            {
                var startInfo = new ProcessStartInfo(XMLConfig.ReadPatchSwitcher());
                startInfo.WorkingDirectory = XMLConfig.ReadGamePath();
                Process.Start(startInfo);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Failed to open Patch Switcher: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Edit()
        {
            var addEditWindow = new AddEditWindow("Edit Mod", SelectedModName);
            addEditWindow.addEditViewModel.RefreshMainResourcesAction = new Action(RefreshMainResources);
            addEditWindow.ShowDialog();
        }

        private void Remove()
        {
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedModName}?", "Delete Mod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (CurrentModName == SelectedModName)
                    {
                        XMLConfig.SetCurrentModName(null);
                        CurrentModName = null;
                        OnPropertyChanged("CurrentModName");
                    }
                    XMLConfig.RemoveMod(SelectedModName);
                    ModNameList.Remove(SelectedModName);
                }
                catch(Exception e)
                {
                    MessageBox.Show($"Failed to delete mod: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void About()
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void Settings()
        {
            var gamePathWindow = new GamePathWindow();
            gamePathWindow.ShowDialog();
        }

        private string GetGamePath(Mod mod)
        {
            if (mod.OverrideGamePath)
            {
                return mod.GamePath;
            }
            else
            {
                return XMLConfig.ReadGamePath();
            }
        }

        public ICommand StartGameCommand
        {
            get
            {
                return new DelegateCommand(StartGame);
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(Add);
            }
        }

        public ICommand SetAsCurrentCommand
        {
            get
            {
                return new DelegateCommand(SetAsCurrent);
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(Remove);
            }
        }

        public ICommand AboutCommand
        {
            get
            {
                return new DelegateCommand(About);
            }
        }

        public ICommand SettingsCommand
        {
            get
            {
                return new DelegateCommand(Settings);
            }
        }

        public ICommand PatchSwitcherCommand
        {
            get
            {
                return new DelegateCommand(PatchSwitcher);
            }
        }
        #endregion Commands

        #region Methods
        private void RefreshMainResources()
        {
            string currentModName = null;
            XMLConfig.ReadXML(ModNameList, ref currentModName);
            CurrentModName = currentModName;
        }
        #endregion
    }
}
