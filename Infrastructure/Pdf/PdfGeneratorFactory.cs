using Core.Interfaces;
using Core.Models;
using Infrastructure.Integrations.Interfaces;
using Infrastructure.Pdf.Generators;
using Infrastructure.Pdf.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Pdf;

public class PdfGeneratorFactory(IEnumerable<IPdfGenerator> generators) : IPdfGeneratorFactory
{
    public IPdfGenerator GetGenerator(InvoiceType type)
    {
        return generators.FirstOrDefault(s => s.CanHandle(type))
               ?? throw new NotSupportedException($"Подходящая для обработки движок - не найдена.");
    }
}