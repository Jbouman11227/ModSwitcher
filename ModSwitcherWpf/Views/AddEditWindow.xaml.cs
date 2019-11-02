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

        public AddEditWindow(string windowName, string selectedMod, List<string> versionNames)
        {
            InitializeComponent();

            addEditViewModel = new AddEditViewModel(windowName, selectedMod
                                                    , versionNames, Close);
            DataContext = addEditViewModel;
        }
    }
}
