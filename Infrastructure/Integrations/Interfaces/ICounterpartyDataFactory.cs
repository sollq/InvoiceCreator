using Core.Models;

namespace Infrastructure.Integrations.Interfaces;

public interface ICounterpartyDataFactory
{
    ICounterpartyDataStrategy GetDataStrategy(InvoiceType type);
}