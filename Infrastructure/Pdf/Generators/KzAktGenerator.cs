using Core.Models;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf.Generators;

public class KzAktGenerator : IPdfGenerator
{
    public byte[] Generate(InvoiceData data)
    {
        throw new NotImplementedException();
    }

    public bool CanHandle(InvoiceType type)
    {
        return type is InvoiceType.KzAkt;
    }
}