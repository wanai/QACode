using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace SalesAnalysis
{
    public abstract class CommandBase:ISalesCommand,ICommand
    {
        static CommandBase()
        {
        }

       
        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute();

         public abstract void Execute();

       bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        public void Execute(object parameter)
        {
            Execute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
