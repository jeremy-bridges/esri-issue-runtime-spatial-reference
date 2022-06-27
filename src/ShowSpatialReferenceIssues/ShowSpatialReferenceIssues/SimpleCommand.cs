using System;
using System.Windows.Input;

namespace ShowSpatialReferenceIssues
{
    public class SimpleCommand : ICommand
    {
        private readonly Func<object?, bool> _canExecute;
        private readonly Action<object?> _execute;

        public SimpleCommand(Func<object?, bool> canExecute, Action<object?> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public SimpleCommand(Action<object?> execute)
            : this(param => true, execute)
        {
        }

        public SimpleCommand(Action execute)
            : this(param => true, param => execute())
        {
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
