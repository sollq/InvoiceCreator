using Core.Models;
using Infrastructure.Integrations.Interfaces;

namespace Infrastructure.Integrations;

public class PkAdataStrategy : ICounterpartyDataStrategy
{
    public Task<SupplierInfo> GetData(string id)
    {
        throw new NotImplementedException();
    }
}