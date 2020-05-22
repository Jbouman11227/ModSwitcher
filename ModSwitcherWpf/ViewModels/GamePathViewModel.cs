using System;
using System.Windows.Input;
using ModSwitcherLib;
using System.Windows.Forms;

namespace ModSwitcherWpf.ViewModels
{
    public class GamePathViewModel : ViewModelBase
    {
        #region Constructors
        public GamePathViewModel()
        {

        }

        public GamePathViewModel(Action closeAction)
        {
            GamePath = null;
            ClickedOK = false;
            CloseAction = closeAction;

            try
            {
                GamePath = XMLConfig.ReadGamePath();
            }
            catch(Exception e)
            {
                MessageBox.Show($"Failed to load default game path: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseAction?.Invoke();
            }
        }
        #endregion

        #region Properties
        public Action CloseAction;

        private string _gamePath;

        public string GamePath
        {
            get
            {
                return _gamePath;
            }
            set
            {
                _gamePath = value;
                OnPropertyChanged("OKEnabled");
                OnPropertyChanged("GamePath");
            }
        }

        public bool OKEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(GamePath);
            }
        }

        public bool ClickedOK { get; set; }
        #endregion

        #region Commands
        private void OpenFolderDialog()
        {
            var folderBrowseDialog = new FolderBrowserDialog();
            folderBrowseDialog.Description = "Select the Game Path:";
            var result = folderBrowseDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                GamePath = folderBrowseDialog.SelectedPath;
            }
        }

        private void OK()
        {
            ClickedOK = true;

            try
            {
                XMLConfig.SetGamePath(GamePath);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Failed to set default game path: {e.Message.AddPeriod()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GamePath = null;
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
