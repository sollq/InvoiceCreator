using System.Runtime.CompilerServices;
using Core.Models;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Desktop.ViewModels;

public class InvoiceInputViewModels : BaseViewModel
{
    private readonly ILogger<InvoiceInputViewModels> _logger;
    private readonly IInvoiceOrchestrator _orchestrator;

    public Array OrganizationTypes { get; } = Enum.GetValues(typeof(OrganizationType));

    private OrganizationType _selectedOrgType = OrganizationType.Ru;
    public OrganizationType SelectedOrgType
    {
        get => _selectedOrgType;
        set
        {
            if (SetProperty(ref _selectedOrgType, value))
            {
                // Можно обновлять номер счета, если нужно
            }
        }
    }

    private string? _companyINN;
    private string? _companyName;
    private string? _companyAddress;
    private string? _contractNumber;
    private DateTime _contractDate = DateTime.Today;
    private bool _isBusy;

    public AsyncRelayCommand CreateInvoiceCommand { get; }
    public AsyncRelayCommand LoadCompanyDataCommand { get; }
    public ProductViewModel ProductVM { get; }

    public InvoiceInputViewModels(
        ILogger<InvoiceInputViewModels> logger,
        ProductViewModel productViewModel,
        IInvoiceOrchestrator orchestrator)
    {
        _logger = logger;
        _orchestrator = orchestrator;
        ProductVM = productViewModel;
        CreateInvoiceCommand = new AsyncRelayCommand(async _ => await CreateInvoice(), _ => CanCreateInvoice());
        LoadCompanyDataCommand = new AsyncRelayCommand(async _ => await LoadCompanyData(), _ => CanLoadCompanyData());
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

    private void UpdateCommands()
    {
        if (!IsBusy)
        {
            CreateInvoiceCommand.RaiseCanExecuteChanged();
            LoadCompanyDataCommand.RaiseCanExecuteChanged();
        }
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
                ContractNumber = ContractNumber ?? string.Empty,
                ContractDate = ContractDate,
                Products = ProductVM.Products.ToList()
            };
            var path = await _orchestrator.CreateInvoiceAsync(input);
            _logger.LogInformation("Счет успешно создан и сохранен: {Path}", path);
            // Можно показать Snackbar/MessageBox
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
            UpdateCommands();
        }
    }

    public async Task InitAsync()
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