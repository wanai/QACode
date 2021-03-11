using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace SalesAnalysis
{        
    public interface ISalesCommand<T> : ICommand
    {
        bool CanExecute(T parameter = default(T));

        void Execute(T parameter = default(T));

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        void RaiseCanExecuteChanged();
    }

    public interface ISalesCommand : ICommand
    {
        bool CanExecute();

        void Execute();

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        void RaiseCanExecuteChanged();
    }

}
