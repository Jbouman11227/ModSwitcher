using System;
using System.Windows;
using ModSwitcherWpf.ViewModels;
using System.Collections.ObjectModel;
using ModSwitcherLib;

namespace ModSwitcherWpf
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

            addEditViewModel = new AddEditViewModel(windowName, selectedMod);
            addEditViewModel.CloseEvent = new Action(Close);
            DataContext = addEditViewModel;
        }
    }
}
