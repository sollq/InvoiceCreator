using Core.Models;

namespace Infrastructure.Pdf.Interfaces;

public interface IPdfGenerator
{
    byte[] Generate(InvoiceData data);

    bool CanHandle(InvoiceType type);
}