using Core.Models;

namespace Core.Interfaces;

public interface IInvoiceNumberCounterService
{
    string GetNextNumber(DocumentType org);
    string SetNumber(DocumentType org, string inputInvoiceNumber);
}