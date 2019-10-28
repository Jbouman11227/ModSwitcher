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
    public class GamePathViewModel : ViewModelBase
    {
        #region Constructors
        public GamePathViewModel()
        {

        }

        public GamePathViewModel(bool firstTime)
        {
            if (firstTime)
            {
                GamePath = null;
            }
            else
            {
                GamePath = XMLConfig.ReadGamePath();
            }
            ClickedOK = false;
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
                return !string.IsNullOrEmpty(GamePath);
            }
        }

        public bool ClickedOK { get; set; }
        #endregion

        #region Commands
        private void OpenFileDialog()
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            var result = openFiledialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                GamePath = openFiledialog.FileName;
            }
        }

        private void OK()
        {
            ClickedOK = true;
            XMLConfig.SetGamePath(GamePath);
            if(CloseAction != null)
            {
                CloseAction();
            }
        }

        public ICommand OpenFileDialogCommand
        {
            get
            {
                return new DelegateCommand(OpenFileDialog);
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
