using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Desktop.ViewModels;

public class InvoiceInputViewModels : BaseViewModel
{
    private readonly ILogger<InvoiceInputViewModels> _logger;

    private string? _companyINN;
    private string? _companyName;
    private DateTime _contractDate = DateTime.Today;
    private string? _contractNumber;
    private bool _isBusy;

    public InvoiceInputViewModels(ILogger<InvoiceInputViewModels> logger)
    {
        _logger = logger;
        CreateInvoiceCommand = new AsyncRelayCommand(async _ => await CreateInvoice(), _ => CanCreateInvoice());
        LoadCompanyDataCommand = new AsyncRelayCommand(async _ => await LoadCompanyData(), _ => CanLoadCompanyData());
    }

    public AsyncRelayCommand CreateInvoiceCommand { get; }
    public AsyncRelayCommand LoadCompanyDataCommand { get; }

    public string? CompanyINN
    {
        get => _companyINN;
        set
        {
            if (SetProperty(ref _companyINN, value))
            {
                CreateInvoiceCommand.RaiseCanExecuteChanged();
                LoadCompanyDataCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? CompanyName
    {
        get => _companyName;
        set => SetProperty(ref _companyName, value);
    }

    public string? ContractNumber
    {
        get => _contractNumber;
        set
        {
            if (SetProperty(ref _contractNumber, value)) CreateInvoiceCommand.RaiseCanExecuteChanged();
        }
    }

    public DateTime ContractDate
    {
        get => _contractDate;
        set
        {
            if (SetProperty(ref _contractDate, value)) CreateInvoiceCommand.RaiseCanExecuteChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private bool CanCreateInvoice()
    {
        return !IsBusy &&
               !string.IsNullOrWhiteSpace(CompanyINN) &&
               !string.IsNullOrWhiteSpace(ContractNumber) &&
               ContractDate != default;
    }

    private bool CanLoadCompanyData()
    {
        return !IsBusy && !string.IsNullOrWhiteSpace(CompanyINN);
    }

    private async Task CreateInvoice()
    {
        try
        {
            IsBusy = true;
            _logger.LogInformation("Создание счета для компании {CompanyINN}", CompanyINN);

            // TODO: Реализовать создание счета
            await Task.Delay(1000); // Имитация работы

            _logger.LogInformation("Счет успешно создан");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании счета");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCompanyData()
    {
        try
        {
            IsBusy = true;
            _logger.LogInformation("Загрузка данных компании {CompanyINN}", CompanyINN);

            // TODO: Интеграция с Dadata/pk.adata.kz
            await Task.Delay(500); // Имитация API-запроса

            // Временные данные для демонстрации
            CompanyName = $"ООО Тестовая Компания ({CompanyINN})";

            _logger.LogInformation("Данные компании загружены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных компании");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task InitAsync()
    {
        try
        {
            await LoadLastNumber();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации");
            throw;
        }
    }

    private async Task LoadLastNumber()
    {
        // TODO: Загрузка последнего номера счета
        await Task.CompletedTask;
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