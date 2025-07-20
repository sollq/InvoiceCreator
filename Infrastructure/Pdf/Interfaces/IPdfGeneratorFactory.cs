using Core.Models;

namespace Infrastructure.Pdf.Interfaces;

public interface IPdfGeneratorFactory
{
    IInvoicePdfGenerator GetGenerator(InvoiceType type);
}