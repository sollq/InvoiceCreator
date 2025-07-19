using Microsoft.Extensions.DependencyInjection;

namespace Core.Interfaces;

public interface IServiceRegistration
{
    public void ConfigureServices(IServiceCollection provider);
}