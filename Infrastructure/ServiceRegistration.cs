using Core.Interfaces;
using Infrastructure.Pdf;
using Infrastructure.Pdf.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class ServiceRegistration : IServiceRegistration
{
    public void ConfigureServices(IServiceCollection services, IConfigurationRoot config)
    {
        services.AddSingleton<INumberToWordsConverter, NumberToWordsConverter>();
        services.AddSingleton<IInvoiceNumberCounterService, InvoiceNumberCounterService>();
        services.AddScoped<KzInvoicePdfGenerator>();
        services.AddScoped<RuInvoicePdfGenerator>();
        services.AddScoped<IInvoicePdfGeneratorFactory, InvoicePdfGeneratorFactory>();
        services.AddSingleton<IMyCompanyInfoProvider, MyCompanyInfoProvider>();
        services.AddSingleton<IInvoiceSaveService, InvoiceSaveService>();
        services.AddTransient<IInvoiceOrchestrator, InvoiceOrchestrator>();
    }
} 