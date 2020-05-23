using System;
using System.Windows.Input;
using ModSwitcherLib;
using System.Windows.Forms;

namespace ModSwitcherWpf.ViewModels
{
    public class GameFolderViewModel : ViewModelBase
    {
        #region Constructors
        public GameFolderViewModel()
        {

        }

        public GameFolderViewModel(Action closeAction)
        {
            GameFolder = null;
            ClickedOK = false;
            CloseAction = closeAction;

            try
            {
                GameFolder = XMLConfig.ReadGameFolder();
            }
            catch(Exception e)
            {
                MessageBox.Show($"Failed to load default game folder: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseAction?.Invoke();
            }
        }
        #endregion

        #region Properties
        public Action CloseAction;

        private string _gameFolder;

        public string GameFolder
        {
            get
            {
                return _gameFolder;
            }
            set
            {
                _gameFolder = value;
                OnPropertyChanged("OKEnabled");
                OnPropertyChanged("GameFolder");
            }
        }

        public bool OKEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(GameFolder);
            }
        }

        public bool ClickedOK { get; set; }
        #endregion

        #region Commands
        private void OpenFolderDialog()
        {
            var folderBrowseDialog = new FolderBrowserDialog();
            folderBrowseDialog.Description = "Select the Game Folder:";
            var result = folderBrowseDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                GameFolder = folderBrowseDialog.SelectedPath;
            }
        }

        private void OK()
        {
            ClickedOK = true;

            try
            {
                XMLConfig.SetGameFolder(GameFolder);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Failed to set default game folder: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GameFolder = null;
                ClickedOK = false;
                return;
            }

            CloseAction?.Invoke();
        }

        public ICommand OpenFolderDialogCommand
        {
            get
            {
                return new DelegateCommand(OpenFolderDialog);
            }
        }

        public ICommand OKCommand
        {
            get
            {
                return new DelegateCommand(OK);
            }
        }
        #endregion
    }
}
