using Core.Models;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf;

public class KzInvoicePdfGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(InvoiceData data)
    {
        throw new NotImplementedException();
    }
}