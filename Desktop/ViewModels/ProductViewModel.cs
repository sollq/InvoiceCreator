using System.Collections.ObjectModel;
using System.Windows.Input;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Desktop.ViewModels;

public class ProductViewModel : BaseViewModel
{
    private readonly ILogger<ProductViewModel> _logger;
    
    public ObservableCollection<Product> Products { get; set; } = [];

    private Product? _selectedProduct;
    private bool _isBusy;

    public AsyncRelayCommand AddCommand { get; }
    public AsyncRelayCommand EditCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    public ProductViewModel(ILogger<ProductViewModel> logger)
    {
        _logger = logger;
        AddCommand = new AsyncRelayCommand(async _ => await Add(), _ => CanAdd());
        EditCommand = new AsyncRelayCommand(async c => await Edit(c as Product), _ => CanEdit());
        DeleteCommand = new AsyncRelayCommand(async c => await Delete(c as Product), _ => CanDelete());
    }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value))
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
            
            // TODO: Загрузка существующих продуктов
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации ProductViewModel");
            throw;
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
            
            // TODO: Открыть диалог добавления продукта
            var newProduct = new Product
            {
                Name = "Новый продукт",
                Quantity = 1,
                Price = 0
            };
            
            Products.Add(newProduct);
            SelectedProduct = newProduct;
            
            _logger.LogInformation("Продукт добавлен: {ProductName}", newProduct.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении продукта");
            throw;
        }
        finally
        {
            IsBusy = false;
            UpdateCommands();
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
            
            // TODO: Открыть диалог редактирования продукта
            await Task.Delay(100); // Имитация работы
            
            _logger.LogInformation("Продукт отредактирован: {ProductName}", product.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при редактировании продукта {ProductName}", product.Name);
            throw;
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
            
            // TODO: Подтверждение удаления
            Products.Remove(product);
            
            if (SelectedProduct == product)
                SelectedProduct = null;
            
            _logger.LogInformation("Продукт удален: {ProductName}", product.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении продукта {ProductName}", product.Name);
            throw;
        }
        finally
        {
            IsBusy = false;
            UpdateCommands();
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