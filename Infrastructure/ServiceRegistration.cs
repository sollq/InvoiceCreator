using Core.Interfaces;
using Infrastructure.Pdf;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class ServiceRegistration : IServiceRegistration
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IInvoiceNumberCounterService, InvoiceNumberCounterService>();
        services.AddScoped<IInvoicePdfGenerator, KzInvoicePdfGenerator>();
        services.AddScoped<IInvoicePdfGenerator, RuInvoicePdfGenerator>();
        services.AddScoped<IInvoicePdfGeneratorFactory, InvoicePdfGeneratorFactory>();
    }
} 