using Core.Models;

namespace Core.Interfaces;

public interface IPdfOrchestrator
{
    Task<string> CreateInvoiceAsync(InvoiceInput input);
}