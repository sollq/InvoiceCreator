using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Desktop.ViewModels;

public class InvoiceInputViewModels : BaseViewModel
{
    private readonly ILogger<InvoiceInputViewModels> _logger;
    
    private string? _companyINN;
    private string? _companyName;
    private string? _contractNumber;
    private DateTime _contractDate = DateTime.Today;
    private bool _isBusy;

    public AsyncRelayCommand CreateInvoiceCommand { get; }
    public AsyncRelayCommand LoadCompanyDataCommand { get; }
    public ProductViewModel ProductVM { get; }

    public InvoiceInputViewModels(ILogger<InvoiceInputViewModels> logger, ProductViewModel productViewModel)
    {
        _logger = logger;
        ProductVM = productViewModel;
        CreateInvoiceCommand = new AsyncRelayCommand(async _ => await CreateInvoice(), _ => CanCreateInvoice());
        LoadCompanyDataCommand = new AsyncRelayCommand(async _ => await LoadCompanyData(), _ => CanLoadCompanyData());
    }

    public string? CompanyINN 
    { 
        get => _companyINN;
        set => SetProperty(ref _companyINN, value);
    }

    public string? CompanyName 
    { 
        get => _companyName;
        set => SetProperty(ref _companyName, value);
    }

    public string? ContractNumber 
    { 
        get => _contractNumber;
        set => SetProperty(ref _contractNumber, value);
    }

    public DateTime ContractDate 
    { 
        get => _contractDate;
        set => SetProperty(ref _contractDate, value);
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