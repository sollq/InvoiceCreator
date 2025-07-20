using Core.Models;

namespace Infrastructure.Integrations.Interfaces;

public interface IPartyInfoStrategy
{
    Task<ClientInfo> GetData(string id);
    bool CanHandle(InvoiceType type);
}