using Core.Models;

namespace Core.Interfaces;

public interface IInvoiceNumberCounterService
{
    string GetNextNumber(OrganizationType org);
    string SetNextNumber(OrganizationType org);
}