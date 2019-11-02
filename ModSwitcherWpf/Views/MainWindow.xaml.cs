using System.Windows;
using System.Windows.Input;
using ModSwitcherWpf.ViewModels;
using System.Windows.Media;
using System.Windows.Controls;

namespace ModSwitcherWpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel mainViewModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            mainViewModel = new MainViewModel(Close);
            DataContext = mainViewModel;
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != listBox)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    mainViewModel.Edit();
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
        }
    }
}
