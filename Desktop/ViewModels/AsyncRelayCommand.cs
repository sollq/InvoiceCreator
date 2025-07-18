using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace Desktop.ViewModels;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Predicate<object?>? _canExecute;
    private readonly ILogger<AsyncRelayCommand>? _logger;
    private bool _isExecuting;

    public AsyncRelayCommand(
        Func<object?, Task> execute, 
        Predicate<object?>? canExecute = null,
        ILogger<AsyncRelayCommand>? logger = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
        _logger = logger;
    }

    public bool CanExecute(object? parameter)
    {
        // Не даём повторно запускать команду, пока она не завершилась
        return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            
            _logger?.LogDebug("Выполнение команды с параметром: {Parameter}", parameter);
            await _execute(parameter);
            _logger?.LogDebug("Команда выполнена успешно");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при выполнении команды с параметром: {Parameter}", parameter);
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

