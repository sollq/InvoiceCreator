using Core.Models;

namespace Infrastructure.Integrations.Interfaces;

public interface ICounterpartyDataStrategy
{
    Task<SupplierInfo> GetData(string id);
}