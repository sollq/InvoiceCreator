using System.Runtime.CompilerServices;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Desktop.ViewModels;

public class InvoiceInputViewModels : BaseViewModel
{
    private readonly IInvoiceNumberCounterService _counterService;
    private readonly IInfoResolver _infoResolver;
    private readonly ILogger<InvoiceInputViewModels> _logger;
    private readonly IPdfOrchestrator _pdfOrchestrator;
    private string? _companyAddress;

    private string? _companyINNOrBin;
    private string? _companyName;
    private string? _companyKpp;
    private DateTime _contractDate = DateTime.Today;
    private string? _contractNumber;
    private string? _invoiceNumber;
    private bool _isBusy;

    private DocumentType _selectedOrgType = DocumentType.InvoiceRu;

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

    public Array OrganizationTypes { get; } = Enum.GetValues(typeof(DocumentType));

    public DocumentType SelectedOrgType
    {
        get => _selectedOrgType;
        set
        {
            if (SetProperty(ref _selectedOrgType, value)) UpdateNextInvoiceNumber();
        }
    }

    public AsyncRelayCommand CreateInvoiceCommand { get; }
    public AsyncRelayCommand LoadCompanyDataCommand { get; }
    public ProductViewModel ProductVM { get; }

    public string? CompanyINNOrBIN
    {
        get => _companyINNOrBin;
        set
        {
            if (SetProperty(ref _companyINNOrBin, value)) LoadCompanyDataCommand.RaiseCanExecuteChanged();
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
    public string? CompanyKPP
    {
        get => _companyKpp;
        set => SetProperty(ref _companyKpp, value);
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
            if (SetProperty(ref _contractDate, value)) UpdateCommands();
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
               !string.IsNullOrWhiteSpace(CompanyINNOrBIN) &&
               !string.IsNullOrWhiteSpace(ContractNumber) &&
               ContractDate != default;
    }

    private bool CanLoadCompanyData()
    {
        return !IsBusy && !string.IsNullOrWhiteSpace(CompanyINNOrBIN);
    }

    private async Task CreateInvoice()
    {
        try
        {
            IsBusy = true;
            var input = new Input
            {
                Type = SelectedOrgType,
                CompanyINN = CompanyINNOrBIN ?? string.Empty,
                CompanyName = CompanyName ?? string.Empty,
                CompanyAddress = CompanyAddress ?? string.Empty,
                InvoiceNumber = InvoiceNumber ?? string.Empty,
                ContractNumber = ContractNumber ?? string.Empty,
                CompanyKPP = CompanyKPP ?? string.Empty,
                ContractDate = ContractDate,
                Products = [.. ProductVM.Products.Where(p => p.IsUsed)]
            };
            var path = await _pdfOrchestrator.CreateInvoiceAsync(input);
            _logger.LogInformation("Счет успешно создан и сохранен: {Path}", path);
            System.Windows.MessageBox.Show("Готово", "Документ", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            UpdateNextInvoiceNumber();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании счета");
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
            if (CompanyINNOrBIN is null) return;
            IsBusy = true;
            _logger.LogInformation("Загрузка данных компании {CompanyINNOrBIN}", CompanyINNOrBIN);
            var info = await _infoResolver.GetPartyInfo(SelectedOrgType, CompanyINNOrBIN);

            CompanyName = info.Name;
            CompanyAddress = info.Address;
            CompanyKPP = info.KPP;
            ContractNumber ??= "1";
            _logger.LogInformation("Данные компании загружены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке данных компании");
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

    private static async Task LoadLastNumber()
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