using System.Windows;
using ModSwitcherWpf.ViewModels;

namespace ModSwitcherWpf.Views
{
    /// <summary>
    /// Interaction logic for GameFolderWindow.xaml
    /// </summary>
    public partial class GameFolderWindow : Window
    {
        public GameFolderViewModel gameFolderViewModel { get; private set; }

        public GameFolderWindow()
        {
            InitializeComponent();

            gameFolderViewModel = new GameFolderViewModel(Close);
            DataContext = gameFolderViewModel;
        }

        public bool ShowDialogResult()
        {
            ShowDialog();
            return gameFolderViewModel.ClickedOK;
        }
    }
}
