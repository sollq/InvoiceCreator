using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class ServiceRegistration : IServiceRegistration
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IInvoiceNumberCounterService, InvoiceNumberCounterService>();
    }
} 