using System;
using System.Windows.Input;

namespace ModSwitcherWpf
{
    public class DelegateCommand : ICommand
    {
        private Action Command { get; set; }

        public DelegateCommand(Action command)
        {
            Command = command;
        }

        #region ICommand Interface
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Command != null;
        }

        public void Execute(object parameter)
        {
            Command();
        }
        #endregion
    }
}
