using Core.Models;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf;

public class PdfGeneratorFactory(IEnumerable<IPdfGenerator> generators) : IPdfGeneratorFactory
{
    public IPdfGenerator GetGenerator(InvoiceType type)
    {
        return generators.FirstOrDefault(s => s.CanHandle(type))
               ?? throw new NotSupportedException("Подходящая для обработки движок - не найдена.");
    }
}