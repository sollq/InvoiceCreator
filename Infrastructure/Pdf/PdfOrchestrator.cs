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
    public async Task<string> CreateInvoiceAsync(Input input)
    {
        var invoiceNumber = input.InvoiceNumber;

        var invoiceData = new DocumentData
        {
            InvoiceNumber = invoiceNumber,
            Date = input.ContractDate,
            Seller = companyProvider.GetInfo(input.Type),
            Buyer = new ClientInfo
            {
                INN = input.CompanyINN,
                Name = input.CompanyName,
                Address = input.CompanyAddress,
                KPP = input.CompanyKPP
            },
            ContractNumber = input.ContractNumber,
            Products = input.Products,
            TotalAmount = input.Products.Sum(p => p.Total),
            TotalAmountText = numberToWordsConverter.Convert(input.Products.Sum(p => p.Total), input.Type),
            OrgType = input.Type
        };

        var generator = factory.GetGenerator(input.Type);
        var pdfBytes = generator.Generate(invoiceData);

        var savePath = saveService.GetSavePath(input.Type.ToString(), invoiceNumber, config);
        await saveService.SaveAsync(savePath, pdfBytes);

        counterService.SetNumber(input.Type, input.InvoiceNumber);

        return savePath;
    }
}