using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace Desktop.ViewModels;

public class AsyncRelayCommand(
    Func<object?, Task> execute,
    Predicate<object?>? canExecute = null,
    ILogger<AsyncRelayCommand>? logger = null)
    : ICommand
{
    private readonly Func<object?, Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private bool _isExecuting;

    public bool CanExecute(object? parameter)
    {
        // Не даём повторно запускать команду, пока она не завершилась
        return !_isExecuting && (canExecute?.Invoke(parameter) ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            logger?.LogDebug("Выполнение команды с параметром: {Parameter}", parameter);
            await _execute(parameter);
            logger?.LogDebug("Команда выполнена успешно");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Ошибка при выполнении команды с параметром: {Parameter}", parameter);
            throw;
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}