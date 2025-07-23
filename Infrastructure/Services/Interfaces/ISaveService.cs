using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Interfaces;

public interface ISaveService
{
    string GetSavePath(string name, string number, IConfiguration config);
    Task SaveAsync(string path, byte[] data);
}