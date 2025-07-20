using Core.Models;

namespace Infrastructure.Integrations.Interfaces;

public interface IInfoResolver
{
    public SupplierInfo GetPartyInfo(InvoiceType type, string id);
}