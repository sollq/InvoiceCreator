using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Core.Models;
using Microsoft.Extensions.Logging;
using Desktop.Views;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace Desktop.ViewModels;

public class ProductViewModel : BaseViewModel
{
    private readonly ILogger<ProductViewModel> _logger;
    private bool _isBusy;

    private Product? _selectedProduct;

    public ProductViewModel(ILogger<ProductViewModel> logger)
    {
        _logger = logger;
        AddCommand = new AsyncRelayCommand(async _ => await Add(), _ => CanAdd());
        EditCommand = new AsyncRelayCommand(async c => await Edit(c as Product), _ => CanEdit());
        DeleteCommand = new AsyncRelayCommand(async c => await Delete(c as Product), _ => CanDelete());
    }

    public ObservableCollection<Product> Products { get; set; } = [];

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

            // TODO: Загрузка существующих продуктов
            await Task.CompletedTask;
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
                // Генерируем Id (можно заменить на свою логику)
                result.Product.Id = Products.Count > 0 ? Products.Max(p => p.Id) + 1 : 1;
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
            await Task.CompletedTask;
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
                // Вот тут твоя проблема: ты напрямую меняешь свойства объекта Product,
                // но если твой ObservableCollection<Product> не знает, что объект внутри изменился,
                // и Product не реализует INotifyPropertyChanged — UI просто не узнает об изменениях.
                // Это классика жанра, и ты в неё вляпался.

                // Если Product реализует INotifyPropertyChanged — убедись, что сеттеры вызывают OnPropertyChanged.
                // Если нет — UI не обновится, хоть ты тресни.

                // Грязный, но рабочий хак: полностью заменить объект в коллекции, чтобы триггернуть обновление.
                // Это не оптимально, но хотя бы честно работает.

                int index = Products.IndexOf(product);
                if (index >= 0)
                {
                    var updatedProduct = new Product
                    {
                        Id = product.Id,
                        Name = result.Product.Name,
                        Quantity = result.Product.Quantity,
                        Price = result.Product.Price,
                        Code = result.Product.Code,
                        Unit = result.Product.Unit
                    };
                    Products[index] = updatedProduct;
                    SelectedProduct = null;
                    SelectedProduct = updatedProduct;
                }
                else
                {
                    // Если по какой-то причине не нашли — fallback на старую логику (но это уже костыль)
                    product.Name = result.Product.Name;
                    product.Quantity = result.Product.Quantity;
                    product.Price = result.Product.Price;
                    product.Code = result.Product.Code;
                    product.Unit = result.Product.Unit;
                    SelectedProduct = null;
                    SelectedProduct = product;
                }

                _logger.LogInformation("Продукт отредактирован: {ProductName}", result.Product.Name);
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

            // TODO: Подтверждение удаления
            Products.Remove(product);

            if (SelectedProduct == product)
                SelectedProduct = null;

            _logger.LogInformation("Продукт удален: {ProductName}", product.Name);
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
            await Task.CompletedTask;
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