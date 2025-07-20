using Core.Models;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf.Generators;

public class KzAktGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(InvoiceData data)
    {
        throw new NotImplementedException();
    }
}