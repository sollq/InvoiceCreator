using Core.Models;

namespace Infrastructure.Integrations.Interfaces;

public interface ICounterpartyDataStrategy
{
    SupplierInfo GetData(string id);
}