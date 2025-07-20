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
        services.AddScoped<IPdfGenerator, KzPdfGenerator>();
        services.AddScoped<IPdfGenerator, RuGenerator>();
        services.AddScoped<IPdfGenerator, RuAktGenerator>();
        services.AddScoped<IPdfGenerator, KzAktGenerator>();
        services.AddScoped<IPdfGeneratorFactory, PdfGeneratorFactory>();
        services.AddSingleton<IMyCompanyInfoProvider, MyCompanyInfoProvider>();
        services.AddSingleton<ISaveService, SaveService>();
        services.AddTransient<IPdfOrchestrator, PdfOrchestrator>();

        services.AddTransient<IPartyInfoStrategy, RuStrategy>();
        services.AddTransient<IPartyInfoStrategy, KzStrategy>();
        services.AddScoped<ICounterpartyDataFactory, CounterpartyDataFactory>();
        services.AddTransient<IInfoResolver, InfoResolver>();
    }
}