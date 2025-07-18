using System.Windows.Input;

namespace Desktop.ViewModels;

public class RelayCommand(Action<object> execute, Predicate<object>? canExecute = null) : ICommand
{
    private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public bool CanExecute(object? parameter)
    {
        return parameter != null && (canExecute == null || canExecute(parameter));
    }

    public void Execute(object? parameter)
    {
        if (parameter != null) _execute(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}