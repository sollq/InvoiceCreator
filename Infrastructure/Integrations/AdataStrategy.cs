using Core.Models;
using Infrastructure.Integrations.Interfaces;

namespace Infrastructure.Integrations;

public class AdataStrategy : ICounterpartyDataStrategy
{
    public SupplierInfo GetData(string id)
    {
        throw new NotImplementedException();
    }
}