using System;
using System.Windows.Input;

namespace MemoryEmulation.DataContext.MVVMBasics
{
    public class Command : ICommand
    {
        #region Constructor

        public Command(Action<object> action)
        {
            Delegate = action;
        }

        #endregion

        #region Delegates

        public Predicate<object> CanUseDelegate { get; set; }
        public Action<object> Delegate { get; set; }

        #endregion

        #region Executing

        public bool CanExecute(object parameter)
        {
            if (CanUseDelegate != null)
            {
                return CanUseDelegate(parameter);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            Delegate?.Invoke(parameter);
        }

        #endregion
    }
}
