using Core.Models;

namespace Core.Interfaces;

public interface IInvoiceNumberCounterService
{
    string GetNextNumber(DocumentType org);
    string SetNextNumber(DocumentType org);
}