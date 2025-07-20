using Core.Models;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf.Generators;

public class KzAktGenerator : IPdfGenerator
{
    public byte[] Generate(DocumentData data)
    {
        throw new NotImplementedException();
    }

    public bool CanHandle(DocumentType type)
    {
        return type is DocumentType.KzAkt;
    }
}