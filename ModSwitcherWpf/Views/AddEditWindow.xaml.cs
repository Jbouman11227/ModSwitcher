using System.Windows;
using ModSwitcherWpf.ViewModels;
using System.Collections.Generic;

namespace ModSwitcherWpf.Views
{
    /// <summary>
    /// Interaction logic for AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        public AddEditViewModel addEditViewModel { get; private set; }

        public AddEditWindow(string windowName, string selectedMod)
        {
            InitializeComponent();

            addEditViewModel = new AddEditViewModel(windowName, selectedMod, Close);
            DataContext = addEditViewModel;
        }
    }
}
