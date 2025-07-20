using Core.Interfaces;
using Infrastructure.Integrations;
using Infrastructure.Integrations.Interfaces;
using Infrastructure.Pdf;
using Infrastructure.Pdf.Generators;
using Infrastructure.Pdf.Interfaces;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
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
        services.AddScoped<RuInvoiceGenerator>();
        services.AddScoped<RuAktGenerator>();
        services.AddScoped<KzAktGenerator>();
        services.AddScoped<IPdfGeneratorFactory, PdfGeneratorFactory>();
        services.AddSingleton<IMyCompanyInfoProvider, MyCompanyInfoProvider>();
        services.AddSingleton<ISaveService, SaveService>();
        services.AddTransient<IPdfOrchestrator, PdfOrchestrator>();



        services.AddScoped<AdataStrategy>();
        services.AddScoped<PkAdataStrategy>();
        services.AddScoped<ICounterpartyDataFactory, CounterpartyDataFactory>();
        services.AddTransient<IInfoResolver, InfoResolver>();
    }
} 