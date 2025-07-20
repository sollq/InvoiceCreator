using Core.Models;

namespace Core.Interfaces;

public interface IInfoResolver
{
    public Task<SupplierInfo> GetPartyInfo(InvoiceType type, string id);
}