using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces;

public interface IProductStorageService
{
    Task<IEnumerable<Product>> LoadProductsAsync();
    Task SaveProductsAsync(IEnumerable<Product> products);
} 