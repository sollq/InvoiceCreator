using Core.Interfaces;
using Core.Models;
using Infrastructure.Integrations.Interfaces;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Integrations;

public class InfoResolver(ICounterpartyDataFactory factory) : IInfoResolver
{
    public async Task<SupplierInfo> GetPartyInfo(InvoiceType type, string id)
    {
        var generator = factory.GetDataStrategy(type);
        return await generator.GetData(id);
    }
}