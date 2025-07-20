using Core.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class SaveService : ISaveService
{
    public string GetSavePath(string invoiceNumber, IConfiguration config)
    {
        // Пример: путь из appsettings.json или стандартная папка
        var folder = config["InvoiceSaveFolder"] ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var fileName = $"Invoice_{invoiceNumber}.pdf";
        return Path.Combine(folder, fileName);
    }

    public async Task SaveAsync(string path, byte[] data)
    {
        await File.WriteAllBytesAsync(path, data);
    }
} 