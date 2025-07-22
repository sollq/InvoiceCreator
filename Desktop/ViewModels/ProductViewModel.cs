using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Core.Models;
using Microsoft.Extensions.Logging;
using Desktop.Views;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;

namespace Desktop.ViewModels;

public class ProductViewModel : BaseViewModel
{
    private readonly ILogger<ProductViewModel> _logger;
    private readonly IProductStorageService _productStorage;
    private bool _isBusy;
    private bool? _allProductsSelected = true;

    private Product? _selectedProduct;

    public ProductViewModel(ILogger<ProductViewModel> logger, IProductStorageService productStorage)
    {
        _logger = logger;
        _productStorage = productStorage;
        AddCommand = new AsyncRelayCommand(async _ => await Add(), _ => CanAdd());
        EditCommand = new AsyncRelayCommand(async c => await Edit(c as Product), _ => CanEdit());
        DeleteCommand = new AsyncRelayCommand(async c => await Delete(c as Product), _ => CanDelete());

        Products.CollectionChanged += async (s, e) => await OnProductsChanged();
    }

    public ObservableCollection<Product> Products { get; set; } = [];

    public decimal TotalAmount => Products.Where(p => p.IsUsed).Sum(p => p.Total);

    public bool? AllProductsSelected
    {
        get => _allProductsSelected;
        set
        {
            if (SetProperty(ref _allProductsSelected, value))
            {
                if (value.HasValue)
                {
                    foreach (var product in Products)
                    {
                        product.IsUsed = value.Value;
                    }
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }
    }

    public AsyncRelayCommand AddCommand { get; }
    public AsyncRelayCommand EditCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value)) UpdateCommands();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private void UpdateCommands()
    {
        // Обновляем команды только если не заняты
        if (!IsBusy)
        {
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    private bool CanAdd()
    {
        return !IsBusy;
    }

    private bool CanEdit()
    {
        return !IsBusy && SelectedProduct != null;
    }

    private bool CanDelete()
    {
        return !IsBusy && SelectedProduct != null;
    }

    public async Task InitAsync()
    {
        try
        {
            IsBusy = true;
            _logger.LogInformation("Инициализация ProductViewModel");

            var loadedProducts = await _productStorage.LoadProductsAsync();
            Products.Clear();
            foreach (var product in loadedProducts)
            {
                product.PropertyChanged += async (s, e) => await OnProductsChanged();
                Products.Add(product);
            }

            OnPropertyChanged(nameof(TotalAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации ProductViewModel");
            // Больше не бросаем исключение
        }
        finally
        {
            IsBusy = false;
            UpdateCommands();
        }
    }

    private async Task Add()
    {
        try
        {
            IsBusy = true;
            _logger.LogInformation("Добавление нового продукта");

            var vm = new ProductDialogViewModel();
            var result = ProductDialog.ShowDialog(Application.Current.MainWindow, vm);
            if (result is { IsOk: true, Product: not null })
            {
                result.Product.Id = Products.Count > 0 ? Products.Max(p => p.Id) + 1 : 1;
                result.Product.PropertyChanged += async (s, e) => await OnProductsChanged();
                Products.Add(result.Product);
                SelectedProduct = result.Product;
                _logger.LogInformation("Продукт добавлен: {ProductName}", result.Product.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении продукта");
            // Больше не бросаем исключение
        }
        finally
        {
            IsBusy = false;
            UpdateCommands();
            await OnProductsChanged();
        }
    }

    private async Task Edit(Product? product)
    {
        if (product == null)
            return;

        try
        {
            IsBusy = true;
            _logger.LogInformation("Редактирование продукта: {ProductName}", product.Name);

            var vm = new ProductDialogViewModel();
            vm.LoadFromProduct(product);
            var result = ProductDialog.ShowDialog(Application.Current.MainWindow, vm);
            if (result is { IsOk: true, Product: not null })
            {
                product.Name = result.Product.Name;
                product.Quantity = result.Product.Quantity;
                product.Price = result.Product.Price;
                product.Code = result.Product.Code;
                product.Unit = result.Product.Unit;

                _logger.LogInformation("Продукт отредактирован: {ProductName}", result.Product.Name);
                OnPropertyChanged(nameof(TotalAmount));
                await OnProductsChanged();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при редактировании продукта {ProductName}", product?.Name);
            // Больше не бросаем исключение
        }
        finally
        {
            IsBusy = false;
            UpdateCommands();
        }
    }

    private async Task Delete(Product? product)
    {
        if (product == null)
            return;

        try
        {
            IsBusy = true;
            _logger.LogInformation("Удаление продукта: {ProductName}", product.Name);

            product.PropertyChanged -= async (s, e) => await OnProductsChanged();
            Products.Remove(product);

            if (SelectedProduct == product)
                SelectedProduct = null;

            _logger.LogInformation("Продукт удален: {ProductName}", product.Name);
            OnPropertyChanged(nameof(TotalAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении продукта {ProductName}", product.Name);
            // Больше не бросаем исключение
        }
        finally
        {
            IsBusy = false;
            UpdateCommands();
            await OnProductsChanged();
        }
    }

    private async Task OnProductsChanged()
    {
        OnPropertyChanged(nameof(TotalAmount));
        await _productStorage.SaveProductsAsync(Products);
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