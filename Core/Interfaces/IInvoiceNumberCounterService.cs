using Core.Models;

namespace Core.Interfaces;

public interface IInvoiceNumberCounterService
{
    string PeekNextNumber(OrganizationType org);
    string GetNextNumber(OrganizationType org);
}