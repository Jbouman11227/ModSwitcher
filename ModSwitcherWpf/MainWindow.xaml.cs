using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using ModSwitcherWpf.ViewModels;
using ModSwitcherLib;
using System.Windows.Media;
using System.Windows.Controls;

namespace ModSwitcherWpf
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
