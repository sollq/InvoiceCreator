using Core.Models;
using Infrastructure.Integrations.Interfaces;

namespace Infrastructure.Integrations;

public class AdataStrategy : ICounterpartyDataStrategy
{
    public Task<SupplierInfo> GetData(string id)
    {
        throw new NotImplementedException();
    }
}s