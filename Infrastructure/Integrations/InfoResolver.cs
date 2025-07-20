using Core.Models;
using Infrastructure.Integrations.Interfaces;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Integrations;

public class InfoResolver(ICounterpartyDataFactory factory) : IInfoResolver
{
    public SupplierInfo GetPartyInfo(InvoiceType type, string id)
    {
        var generator = factory.GetDataStrategy(type);
        return generator.GetData(id);
    }
}