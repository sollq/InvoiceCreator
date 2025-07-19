using Core.Models;

namespace Infrastructure.Pdf.Interfaces;

public interface IInvoicePdfGeneratorFactory
{
    IInvoicePdfGenerator GetGenerator(OrganizationType type);
}