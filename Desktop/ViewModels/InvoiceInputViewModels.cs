using System.Runtime.CompilerServices;
using Core.Models;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace Desktop.ViewModels;

public class InvoiceInputViewModels : BaseViewModel
{
    private readonly ILogger<InvoiceInputViewModels> _logger;
    private readonly IInvoiceNumberCounterService _counterService;
    private readonly IInvoicePdfGeneratorFactory _factory;
    private readonly MyCompanyInfoProvider _myCompanyInfoProvider;

    public Array OrganizationTypes { get; } = Enum.GetValues(typeof(OrganizationType));

    private OrganizationType _selectedOrgType = OrganizationType.Ru;
    public OrganizationType SelectedOrgType
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
    private string? _contractNumber;
    private DateTime _contractDate = DateTime.Today;
    private bool _isBusy;

    public AsyncRelayCommand CreateInvoiceCommand { get; }
    public AsyncRelayCommand LoadCompanyDataCommand { get; }
    public ProductViewModel ProductVM { get; }

    public InvoiceInputViewModels(
        ILogger<InvoiceInputViewModels> logger,
        ProductViewModel productViewModel,
        IInvoiceNumberCounterService counterService, 
        IInvoicePdfGeneratorFactory factory,
        MyCompanyInfoProvider myCompanyInfoProvider)
    {
        _factory = factory;
        _logger = logger;
        _counterService = counterService;
        _myCompanyInfoProvider = myCompanyInfoProvider;
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
        ContractNumber = _counterService.PeekNextNumber(SelectedOrgType);
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

            if (string.IsNullOrWhiteSpace(ContractNumber) || ContractNumber.StartsWith("СЧЕТ-"))
                ContractNumber = _counterService.GetNextNumber(SelectedOrgType);
            else
                _ = _counterService.GetNextNumber(SelectedOrgType);

            var seller = _myCompanyInfoProvider.GetInfo(SelectedOrgType);

            var invoiceData = new InvoiceData
            {
                InvoiceNumber = ContractNumber,
                Date = ContractDate,
                Seller = seller,
                Buyer = new ClientInfo
                {
                    INN = CompanyINN ?? "",
                    Name = CompanyName ?? "",
                    Address = CompanyAddress ?? ""
                },
                ContractNumber = ContractNumber,
                Products = ProductVM.Products.ToList(),
                TotalAmount = ProductVM.Products.Sum(p => p.Total),
                TotalAmountText = NumberToWordsConverter.Convert(ProductVM.Products.Sum(p => p.Total)) + " тенге",
                OrgType = SelectedOrgType
            };

            var generator = _factory.GetGenerator(SelectedOrgType);
            var pdfBytes = generator.Generate(invoiceData);

            var savePath = GetSavePathForInvoice(ContractNumber);
            if (!string.IsNullOrWhiteSpace(savePath))
                await File.WriteAllBytesAsync(savePath, pdfBytes);

            _logger.LogInformation("Счет успешно создан и сохранен: {Path}", savePath);

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