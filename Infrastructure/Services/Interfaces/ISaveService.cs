using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Interfaces;

public interface ISaveService
{
    string GetSavePath(string invoiceNumber, IConfiguration config);
    Task SaveAsync(string path, byte[] data);
} 