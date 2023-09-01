using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TSPSimulation.UserInterface.Util
{
    internal class AsyncDelegateCommand : ICommand
    {
        private readonly Func<object, Task> executeAsync;
        private readonly Predicate<object> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncDelegateCommand(Func<object, Task> executeAsync, Predicate<object> canExecute = null)
        {
            this.executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public async Task ExecuteAsync(object? parameter)
        {
            await this.executeAsync(parameter);
        }

        async void ICommand.Execute(object? parameter)
        {
            await this.ExecuteAsync(parameter);
        }
    }
}
