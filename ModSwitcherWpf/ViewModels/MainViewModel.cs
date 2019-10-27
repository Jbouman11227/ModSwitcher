using System;
using ModSwitcherLib;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Diagnostics;

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
            CloseEvent = closeAction;

            if (File.Exists("config.xml"))
            {
                try
                {
                    string currentModName = null;
                    XMLConfig.ReadXML(ModNameList, ref currentModName);
                    CurrentModName = currentModName;
                    if (string.IsNullOrEmpty(XMLConfig.ReadGamePath()))
                    {
                        SetGamePathOrTerminate();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to load the configuration from config.xml: {e.Message}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (CloseEvent != null)
                    {
                        CloseEvent();
                    }
                }
            }
            else
            {
                XMLConfig.WriteNewConfig();
                SetGamePathOrTerminate();
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

        public Action CloseEvent;
        #endregion

        #region Commands
        private void StartGame()
        {
            string gamePath = XMLConfig.ReadGamePath();
            Mod CurrentMod = XMLConfig.ReadMod(CurrentModName);

            string flag = string.Empty;
            if (CurrentMod.UsingModPath)
            {
                if (!string.IsNullOrEmpty(CurrentMod.ModPath))
                {
                    flag = $"-mod \"{CurrentMod.ModPath}\"";
                }
            }
            else
            {
                flag = CurrentMod.Flag;
            }
            try
            {
                Process.Start($"\"{gamePath}\"", flag);
                if (CloseEvent != null)
                {
                    CloseEvent();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to start {CurrentMod.ModName}: {e.Message}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Add()
        {
            AddEditWindow addEditWindow = new AddEditWindow("Add Mod", SelectedModName);
            addEditWindow.addEditViewModel.RefreshMainResourcesAction = new Action(RefreshMainResources);
            addEditWindow.ShowDialog();
        }

        private void SetAsCurrent()
        {
            CurrentModName = SelectedModName;
            XMLConfig.SetCurrentModName(SelectedModName);
        }

        public void Edit()
        {
            AddEditWindow addEditWindow = new AddEditWindow("Edit Mod", SelectedModName);
            addEditWindow.addEditViewModel.RefreshMainResourcesAction = new Action(RefreshMainResources);
            addEditWindow.ShowDialog();
        }

        private void Remove()
        {
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedModName}?", "Delete Mod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                if (CurrentModName == SelectedModName)
                {
                    CurrentModName = null;
                    OnPropertyChanged("CurrentModName");
                }
                ModNameList.Remove(SelectedModName);
                XMLConfig.RemoveMod(SelectedModName);
            }
        }

        private void About()
        {

        }

        private void Settings()
        {
            var gamePathWindow = new GamePathWindow(false);
            gamePathWindow.ShowDialog();
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
        #endregion Commands

        private void SetGamePathOrTerminate()
        {
            bool clickedOK = (new GamePathWindow(true)).ShowDialogResult();
            if (!clickedOK)
            {
                CloseEvent();
            }
        }

        private void RefreshMainResources()
        {
            ModNameList.Clear(); string currentModName = null;
            XMLConfig.ReadXML(ModNameList, ref currentModName);
            CurrentModName = currentModName;
        }
    }
}
