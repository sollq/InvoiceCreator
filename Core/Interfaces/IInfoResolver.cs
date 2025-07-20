using Core.Models;

namespace Core.Interfaces;

public interface IInfoResolver
{
    public Task<ClientInfo> GetPartyInfo(InvoiceType type, string id);
}