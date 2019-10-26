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
            CurrentMod = null;
            ModList = new ObservableCollection<Mod>();
            SelectedMod = null;
            CloseEvent = closeAction;

            if (File.Exists("config.xml"))
            {
                try
                {
                    string currentModName = null;
                    XMLConfig.ReadXML(ModList, ref currentModName);
                    CurrentMod = ModList.Where(mod => mod.ModName == currentModName).FirstOrDefault();
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

        private Mod _currentMod;

        public Mod CurrentMod
        {
            get
            {
                return _currentMod;
            }
            set
            {
                _currentMod = value;
                OnPropertyChanged("CurrentMod");
                OnPropertyChanged("StartGameEnabled");
            }
        }

        private Mod _selectedMod;
        public Mod SelectedMod
        {
            get
            {
                return _selectedMod;
            }
            set
            {
                _selectedMod = value;
                OnPropertyChanged("RemoveSetasCurrentEnabled");
            }
        }

        public bool StartGameEnabled
        {
            get
            {
                return CurrentMod != null;
            }
        }

        public bool RemoveSetasCurrentEnabled
        {
            get
            {
                return SelectedMod != null;
            }
        }

        public Action CloseEvent;
        #endregion

        #region Commands
        private void StartGame()
        {
            string gamePath = XMLConfig.ReadGamePath();
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
            AddEditWindow addEditWindow = new AddEditWindow("Add Mod", this);
            addEditWindow.ShowDialog();
        }

        private void SetAsCurrent()
        {
            CurrentMod = SelectedMod;
            XMLConfig.WriteXML(ModList, CurrentMod);
        }

        public void Edit()
        {
            AddEditWindow addEditWindow = new AddEditWindow("Edit Mod", this);
            addEditWindow.ShowDialog();
        }

        private void Remove()
        {
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedMod.ModName}?", "Delete Mod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                if (CurrentMod == SelectedMod)
                {
                    CurrentMod = null;
                    OnPropertyChanged("CurrentMod");
                }
                ModList.Remove(SelectedMod);
                XMLConfig.WriteXML(ModList, CurrentMod);
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
    }
}
