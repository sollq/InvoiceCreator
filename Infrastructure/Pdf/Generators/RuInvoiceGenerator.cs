using Core.Interfaces;
using Core.Models;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf.Generators;

public class RuInvoiceGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(InvoiceData data)
    {
        //var document = Document.Create(container =>
        //{
        //    container.Page(page =>
        //    {
        //        page.Size(PageSizes.A4);
        //        page.Margin(2, Unit.Centimetre);
        //        page.Content()
        //            .Column(col =>
        //            {
        //                col.Item().Text($"Счет № {data.InvoiceNumber}").Bold().FontSize(20);
        //                // ... остальной layout ...
        //            });
        //    });
        //});
        throw new NotImplementedException();
    }
}