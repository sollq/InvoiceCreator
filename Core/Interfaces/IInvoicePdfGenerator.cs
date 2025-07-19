using Core.Models;

namespace Core.Interfaces;

public interface IInvoicePdfGenerator
{
    byte[] Generate(InvoiceData data);
}