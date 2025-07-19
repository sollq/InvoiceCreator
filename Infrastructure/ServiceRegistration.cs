using Core.Interfaces;
using Infrastructure.Pdf;
using Infrastructure.Pdf.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class ServiceRegistration : IServiceRegistration
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<INumberToWordsConverter, NumberToWordsConverter>();
        services.AddTransient<IInvoiceOrchestrator, InvoiceOrchestrator>();
        services.AddSingleton<IInvoiceNumberCounterService, InvoiceNumberCounterService>();
        services.AddScoped<IInvoicePdfGenerator, KzInvoicePdfGenerator>();
        services.AddScoped<IInvoicePdfGenerator, RuInvoicePdfGenerator>();
        services.AddScoped<IInvoicePdfGeneratorFactory, InvoicePdfGeneratorFactory>();
        services.AddSingleton<IMyCompanyInfoProvider, MyCompanyInfoProvider>();
        services.AddSingleton<IInvoiceSaveService, InvoiceSaveService>();
    }
} 