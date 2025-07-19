using Core.Models;

namespace Infrastructure.Pdf.Interfaces;

public interface IInvoicePdfGenerator
{
    byte[] Generate(InvoiceData data);
}