using Core.Interfaces;
using Core.Models;
using Infrastructure.Integrations;
using Infrastructure.Integrations.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Desktop.ViewModels;

public class InvoiceInputViewModels : BaseViewModel
{
    private readonly ILogger<InvoiceInputViewModels> _logger;
    private readonly IInvoiceNumberCounterService _counterService;
    private readonly IPdfOrchestrator _pdfOrchestrator;
    private readonly IInfoResolver _infoResolver;
    public Array OrganizationTypes { get; } = Enum.GetValues(typeof(InvoiceType));

    private InvoiceType _selectedOrgType = InvoiceType.Ru;
    public InvoiceType SelectedOrgType
    {
        get => _selectedOrgType;
        set
        {
            if (SetProperty(ref _selectedOrgType, value))
            {
                UpdateNextInvoiceNumber();
            }
        }
    }

    private string? _companyINN;
    private string? _companyName;
    private string? _companyAddress;
    private string? _contractNumber;
    private string? _invoiceNumber;
    private DateTime _contractDate = DateTime.Today;
    private bool _isBusy;

    public AsyncRelayCommand CreateInvoiceCommand { get; }
    public AsyncRelayCommand LoadCompanyDataCommand { get; }
    public ProductViewModel ProductVM { get; }

    public InvoiceInputViewModels(
        ILogger<InvoiceInputViewModels> logger,
        ProductViewModel productViewModel,
        IInvoiceNumberCounterService counterService, 
        IPdfOrchestrator pdfOrchestrator, 
        IInfoResolver infoResolver)
    {
        _pdfOrchestrator = pdfOrchestrator;
        _infoResolver = infoResolver;
        _logger = logger;
        _counterService = counterService;
        ProductVM = productViewModel;
        CreateInvoiceCommand = new AsyncRelayCommand(async _ => await CreateInvoice(), _ => CanCreateInvoice());
        LoadCompanyDataCommand = new AsyncRelayCommand(async _ => await LoadCompanyData(), _ => CanLoadCompanyData());
        UpdateNextInvoiceNumber();
    }

    public string? CompanyINN 
    { 
        get => _companyINN;
        set
        {
            if (SetProperty(ref _companyINN, value))
            {
                LoadCompanyDataCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? CompanyName 
    { 
        get => _companyName;
        set => SetProperty(ref _companyName, value);
    }
    public string? CompanyAddress
    {
        get => _companyAddress;
        set => SetProperty(ref _companyAddress, value);
    }
    public string? InvoiceNumber
    {
        get => _invoiceNumber;
        set => SetProperty(ref _invoiceNumber, value);
    }
    public string? ContractNumber 
    { 
        get => _contractNumber;
        set => SetProperty(ref _contractNumber, value);
    }

    public DateTime ContractDate 
    { 
        get => _contractDate;
        set
        {
            if (SetProperty(ref _contractDate, value))
            {
                UpdateCommands();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private void UpdateNextInvoiceNumber()
    {
        InvoiceNumber = _counterService.GetNextNumber(SelectedOrgType);
    }

    private void UpdateCommands()
    {
        if (IsBusy) 
            return;
        CreateInvoiceCommand.RaiseCanExecuteChanged();
        LoadCompanyDataCommand.RaiseCanExecuteChanged();
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
            var input = new InvoiceInput
            {
                OrgType = SelectedOrgType,
                CompanyINN = CompanyINN ?? string.Empty,
                CompanyName = CompanyName ?? string.Empty,
                CompanyAddress = CompanyAddress ?? string.Empty,
                InvoiceNumber = InvoiceNumber ?? string.Empty,
                ContractNumber = ContractNumber ?? string.Empty,
                ContractDate = ContractDate,
                Products = [.. ProductVM.Products]
            };
            var path = await _pdfOrchestrator.CreateInvoiceAsync(input);
             _logger.LogInformation("Счет успешно создан и сохранен: {Path}", path);
             UpdateNextInvoiceNumber();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании счета");
            throw;
        }
        finally
        {
            IsBusy = false;
            UpdateCommands();
        }
    }


    private async Task LoadCompanyData()
    {
        try
        {
            IsBusy = true;
            _logger.LogInformation("Загрузка данных компании {CompanyINN}", CompanyINN);
            await Task.Delay(500); // Имитация API-запроса
            await _infoResolver.GetPartyInfo();
            CompanyName = $"ООО Тестовая Компания ({CompanyINN})";
            CompanyAddress = $"Тестовый адресс";
            ContractNumber ??= "1";
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
            UpdateCommands();
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