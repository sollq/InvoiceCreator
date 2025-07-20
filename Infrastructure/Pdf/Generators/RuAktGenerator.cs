using Core.Models;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf.Generators;

public class RuAktGenerator : IPdfGenerator
{
    public bool CanHandle(InvoiceType type)
    {
        return type is InvoiceType.RuAkt;
    }

    public byte[] Generate(InvoiceData data)
    {
        throw new NotImplementedException();
    }
}