using Microsoft.Extensions.Configuration;

namespace Core.Interfaces;

public interface IInvoiceSaveService
{
    string GetSavePath(string invoiceNumber, IConfiguration config);
    Task SaveAsync(string path, byte[] data);
} 