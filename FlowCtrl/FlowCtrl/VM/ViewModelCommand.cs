using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlowCtrl.VM
{
    public class ViewModelCommand : ICommand
    {
        public ViewModelCommand(Action<object> executeAction, Predicate<object> canExecute)
        {
            if (executeAction == null)
                throw new ArgumentNullException("executeAction");
            _canExecute = canExecute;
            _executeAction = executeAction;
        }

        #region ICommand Members
        Predicate<object> _canExecute;
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        Action<object> _executeAction;
        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        #endregion
    }
}
