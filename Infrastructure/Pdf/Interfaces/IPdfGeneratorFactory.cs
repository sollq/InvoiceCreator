using Core.Models;

namespace Infrastructure.Pdf.Interfaces;

public interface IPdfGeneratorFactory
{
    IPdfGenerator GetGenerator(InvoiceType type);
}