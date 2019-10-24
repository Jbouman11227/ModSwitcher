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
        public MainViewModel()
        {
            CurrentMod = null;
            ModList = new ObservableCollection<Mod>();
            SelectedMod = null;
            if (File.Exists("config.xml"))
            {
                try
                {
                    string currentModName = null;
                    XMLConfig.ReadXML(ModList, ref currentModName);
                    CurrentMod = ModList.Where(mod => mod.ModName == currentModName).FirstOrDefault();
                }
                catch(Exception e)
                {
                    MessageBox.Show($"Failed to load the configuration from config.xml: {e.Message}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if(CloseEvent != null)
                    {
                        CloseEvent();
                    }
                } 
            }
        }

        public ObservableCollection<Mod> ModList { get; set; }

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
        
        private void StartGame()
        {
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
                Process.Start($"\"{CurrentMod.GamePath}\"", flag);
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
            if(result == DialogResult.Yes)
            {
                if(CurrentMod == SelectedMod)
                {
                    CurrentMod = null;
                    OnPropertyChanged("CurrentMod");
                }
                ModList.Remove(SelectedMod);
                XMLConfig.WriteXML(ModList, CurrentMod);
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
    }
}
