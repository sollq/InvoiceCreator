using Core.Interfaces;
using Core.Models;
using Infrastructure.Pdf.Generators;
using Infrastructure.Pdf.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Pdf;

public class PdfGeneratorFactory(IServiceProvider provider) : IPdfGeneratorFactory
{
    public IInvoicePdfGenerator GetGenerator(InvoiceType type)
    {
        return type switch
        {
            InvoiceType.Ru => provider.GetRequiredService<RuInvoiceGenerator>(),
            InvoiceType.Kz => provider.GetRequiredService<KzInvoicePdfGenerator>(),
            InvoiceType.RuAkt => provider.GetRequiredService<RuAktGenerator>(),
            InvoiceType.KzAkt => provider.GetRequiredService<KzAktGenerator>(),
            _ => throw new NotSupportedException()
        };
    }
}