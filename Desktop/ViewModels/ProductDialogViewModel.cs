using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Desktop.ViewModels;

public class ProductDialogViewModel : BaseViewModel
{
    private string _name = string.Empty;
    private int _quantity;
    private decimal _price;
    private string _code = string.Empty;
    private string _unit = string.Empty;
    private bool _dialogResult;
    private bool _isCancelled;

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); OkCommand.RaiseCanExecuteChanged(); }
    }

    public int Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); OkCommand.RaiseCanExecuteChanged(); }
    }

    public decimal Price
    {
        get => _price;
        set { _price = value; OnPropertyChanged(); OkCommand.RaiseCanExecuteChanged(); }
    }

    public string Code
    {
        get => _code;
        set { _code = value; OnPropertyChanged(); OkCommand.RaiseCanExecuteChanged(); }
    }

    public string Unit
    {
        get => _unit;
        set { _unit = value; OnPropertyChanged(); OkCommand.RaiseCanExecuteChanged(); }
    }

    public bool DialogResult
    {
        get => _dialogResult;
        private set { _dialogResult = value; OnPropertyChanged(); }
    }

    public bool IsCancelled
    {
        get => _isCancelled;
        private set { _isCancelled = value; OnPropertyChanged(); }
    }

    public AsyncRelayCommand OkCommand { get; }
    public AsyncRelayCommand CancelCommand { get; }

    public ProductDialogViewModel()
    {
        OkCommand = new AsyncRelayCommand(async _ => await OnOk(), _ => CanOk());
        CancelCommand = new AsyncRelayCommand(async _ => await OnCancel());
    }

    private bool CanOk()
    {
        return !string.IsNullOrWhiteSpace(Name)
            && Quantity > 0
            && Price >= 0
            && !string.IsNullOrWhiteSpace(Code)
            && !string.IsNullOrWhiteSpace(Unit);
    }

    private Task OnOk()
    {
        DialogResult = true;
        IsCancelled = false;
        return Task.CompletedTask;
    }

    private Task OnCancel()
    {
        DialogResult = false;
        IsCancelled = true;
        return Task.CompletedTask;
    }

    public void LoadFromProduct(Core.Models.Product? product)
    {
        if (product == null) return;
        Name = product.Name;
        Quantity = product.Quantity;
        Price = product.Price;
        Code = product.Code;
        Unit = product.Unit;
    }

    public Core.Models.Product ToProduct(int? id = null)
    {
        return new Core.Models.Product
        {
            Id = id ?? 0,
            Name = Name,
            Quantity = Quantity,
            Price = Price,
            Code = Code,
            Unit = Unit
        };
    }
} 