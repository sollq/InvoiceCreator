using Core.Interfaces;
using Core.Models;
using Infrastructure.Pdf.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Pdf;

public class PdfOrchestrator(
    IPdfGeneratorFactory factory,
    IInvoiceNumberCounterService counterService,
    IMyCompanyInfoProvider companyProvider,
    ISaveService saveService,
    INumberToWordsConverter numberToWordsConverter,
    IConfiguration config) : IPdfOrchestrator
{
    public async Task<string> CreateInvoiceAsync(InvoiceInput input)
    {
        var invoiceNumber = input.InvoiceNumber;

        var invoiceData = new InvoiceData
        {
            InvoiceNumber = invoiceNumber,
            Date = input.ContractDate,
            Seller = companyProvider.GetInfo(input.OrgType),
            Buyer = new ClientInfo
            {
                INN = input.CompanyINN,
                Name = input.CompanyName,
                Address = input.CompanyAddress
            },
            ContractNumber = input.ContractNumber,
            Products = input.Products,
            TotalAmount = input.Products.Sum(p => p.Total),
            TotalAmountText = numberToWordsConverter.Convert(input.Products.Sum(p => p.Total)) + " тенге",
            OrgType = input.OrgType
        };

        var generator = factory.GetGenerator(input.OrgType);
        var pdfBytes = generator.Generate(invoiceData);

        var savePath = saveService.GetSavePath(invoiceNumber, config);
        await saveService.SaveAsync(savePath, pdfBytes);

        counterService.SetNextNumber(input.OrgType);

        return savePath;
    }
}