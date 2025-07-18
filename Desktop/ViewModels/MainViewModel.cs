using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Desktop.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly ILogger<MainViewModel> _logger;
    private bool _isBusy;

    public InvoiceInputViewModels InvoiceCreationVM { get; }
    public ProductViewModel ProductVM { get; }

    public MainViewModel(
        InvoiceInputViewModels invoiceViewModel, 
        ProductViewModel productViewModel,
        ILogger<MainViewModel> logger)
    {
        _logger = logger;
        InvoiceCreationVM = invoiceViewModel;
        ProductVM = productViewModel;
    }

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
            _logger.LogInformation("Инициализация MainViewModel");
            
            await InvoiceCreationVM.InitAsync();
            await ProductVM.InitAsync();
            
            _logger.LogInformation("MainViewModel успешно инициализирован");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации MainViewModel");
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