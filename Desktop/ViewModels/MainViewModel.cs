using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Desktop.ViewModels;

public class MainViewModel(
    InvoiceInputViewModels invoiceViewModel,
    ProductViewModel productViewModel,
    ILogger<MainViewModel> logger) : BaseViewModel
{
    private bool _isBusy;

    public InvoiceInputViewModels InvoiceCreationVM { get; } = invoiceViewModel;
    public ProductViewModel ProductVM { get; } = productViewModel;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public async Task InitAsync()
    {
        try
        {
            IsBusy = true;
            logger.LogInformation("Инициализация MainViewModel");

            await InvoiceCreationVM.InitAsync();
            await ProductVM.InitAsync();

            logger.LogInformation("MainViewModel успешно инициализирован");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при инициализации MainViewModel");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}