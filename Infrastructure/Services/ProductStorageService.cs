using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ProductStorageService : IProductStorageService
{
    private readonly ILogger<ProductStorageService> _logger;
    private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InvoiceCreator");
    private static readonly string FilePath = Path.Combine(AppDataPath, "products.json");

    public ProductStorageService(ILogger<ProductStorageService> logger)
    {
        _logger = logger;
        Directory.CreateDirectory(AppDataPath);
    }

    public async Task<IEnumerable<Product>> LoadProductsAsync()
    {
        if (!File.Exists(FilePath))
        {
            _logger.LogInformation("Файл с продуктами не найден. Возвращен пустой список.");
            return [];
        }

        try
        {
            var json = await File.ReadAllTextAsync(FilePath);
            var products = JsonSerializer.Deserialize<IEnumerable<Product>>(json);
            _logger.LogInformation("Продукты успешно загружены из {FilePath}", FilePath);
            return products ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке продуктов из {FilePath}", FilePath);
            return [];
        }
    }

    public async Task SaveProductsAsync(IEnumerable<Product> products)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(products, options);
            await File.WriteAllTextAsync(FilePath, json);
            _logger.LogInformation("Продукты ({Count}) успешно сохранены в {FilePath}", products.Count(), FilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении продуктов в {FilePath}", FilePath);
        }
    }
} 