using Core.Models;

namespace Core.Interfaces;

public interface IInvoiceOrchestrator
{
    Task<string> CreateInvoiceAsync(InvoiceInput input);
}