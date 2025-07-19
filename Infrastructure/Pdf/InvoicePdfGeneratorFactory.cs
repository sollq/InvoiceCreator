using Core.Interfaces;
using Core.Models;
using Infrastructure.Pdf.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Pdf;

public class InvoicePdfGeneratorFactory(IServiceProvider provider) : IInvoicePdfGeneratorFactory
{
    public IInvoicePdfGenerator GetGenerator(OrganizationType type)
    {
        return type switch
        {
            OrganizationType.Ru => provider.GetRequiredService<RuInvoicePdfGenerator>(),
            OrganizationType.Kz => provider.GetRequiredService<KzInvoicePdfGenerator>(),
            _ => throw new NotSupportedException()
        };
    }
}