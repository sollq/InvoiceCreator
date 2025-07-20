using Core.Models;

namespace Core.Interfaces;

public interface IInvoiceNumberCounterService
{
    string GetNextNumber(InvoiceType org);
    string SetNextNumber(InvoiceType org);
}