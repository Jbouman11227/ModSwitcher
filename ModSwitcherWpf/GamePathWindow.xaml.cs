using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ModSwitcherWpf.ViewModels;

namespace ModSwitcherWpf
{
    /// <summary>
    /// Interaction logic for GamePathWindow.xaml
    /// </summary>
    public partial class GamePathWindow : Window
    {
        public GamePathViewModel gamePathViewModel { get; private set; }

        public GamePathWindow(bool firstTime)
        {
            InitializeComponent();

            gamePathViewModel = new GamePathViewModel(firstTime);
            gamePathViewModel.CloseAction = new Action(Close);

            DataContext = gamePathViewModel;
        }

        public bool ShowDialogResult()
        {
            ShowDialog();
            return gamePathViewModel.ClickedOK;
        }
    }
}
