using System.Windows;
using ModSwitcherWpf.ViewModels;

namespace ModSwitcherWpf.Views
{
    /// <summary>
    /// Interaction logic for GamePathWindow.xaml
    /// </summary>
    public partial class GamePathWindow : Window
    {
        public GamePathViewModel gamePathViewModel { get; private set; }

        public GamePathWindow()
        {
            InitializeComponent();

            gamePathViewModel = new GamePathViewModel(Close);
            DataContext = gamePathViewModel;
        }

        public bool ShowDialogResult()
        {
            ShowDialog();
            return gamePathViewModel.ClickedOK;
        }
    }
}
