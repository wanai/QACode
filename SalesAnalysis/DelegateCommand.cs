using System;
using System.Windows.Input;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace SalesAnalysis
{
    public class DelegateCommand<T> : ISalesCommand<T>
    {
        public DelegateCommand(Action<T> executeMethod) : this(executeMethod, null)
        { }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            Contract.Ensures(executeMethod != null);

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        private readonly Func<T, bool> canExecuteMethod;
        private readonly Action<T> executeMethod;

        bool ICommand.CanExecute(object parameter)
        {
            if (canExecuteMethod == null)
            {
                return true;
            }
            bool canExecute = false;
           
                canExecute = CanExecute((T)parameter);          

            return canExecute;
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

        public event EventHandler CanExecuteChanged;

        void ICommand.Execute(object parameter)
        {            
            Execute((T)parameter);
        }

         public bool CanExecute(T parameter)
        {
            return (canExecuteMethod == null || canExecuteMethod(parameter));
        }

        public void Execute(T parameter)
        {
            if (executeMethod == null)
            {
                return;
            }

            executeMethod(parameter);
        }
    }

   public class DelegateCommand : CommandBase
    {
       public DelegateCommand(Action executeMethod) : this(executeMethod, null)
        { }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            Contract.Ensures(executeMethod != null);
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        private readonly Func<bool> canExecuteMethod;
        private readonly Action executeMethod;

        public override bool CanExecute()
        {
            if (canExecuteMethod != null)
            {
                return canExecuteMethod();
            }

            return true;
        }

        public override void Execute()
        {
            executeMethod();
        }
    }
}
