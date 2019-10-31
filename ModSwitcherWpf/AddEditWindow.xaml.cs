using System;
using System.Windows;
using ModSwitcherWpf.ViewModels;
using System.Collections.ObjectModel;
using ModSwitcherLib;
using System.Collections.Generic;
namespace ModSwitcherWpf
{
    /// <summary>
    /// Interaction logic for AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        public AddEditViewModel addEditViewModel { get; private set; }

        public AddEditWindow(string windowName, string selectedMod, List<string> versionNames)
        {
            InitializeComponent();

            addEditViewModel = new AddEditViewModel(windowName, selectedMod
                                                    , versionNames, Close);
            DataContext = addEditViewModel;
        }
    }
}
