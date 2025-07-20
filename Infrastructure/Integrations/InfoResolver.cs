using Core.Interfaces;
using Core.Models;
using Infrastructure.Integrations.Interfaces;

namespace Infrastructure.Integrations;

public class InfoResolver(ICounterpartyDataFactory factory) : IInfoResolver
{
    public async Task<ClientInfo> GetPartyInfo(InvoiceType type, string id)
    {
        var generator = factory.GetDataStrategy(type);
        return await generator.GetData(id);
    }
}