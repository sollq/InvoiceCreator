using Core.Models;
using Infrastructure.Integrations.Interfaces;
using Infrastructure.Pdf;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Integrations;

public class CounterpartyDataFactory(IServiceProvider provider) : ICounterpartyDataFactory
{
    private readonly Dictionary<InvoiceType, ICounterpartyDataStrategy> _strategies = new()
    {
        { InvoiceType.Ru, provider.GetRequiredService<AdataStrategy>() },
        { InvoiceType.RuAkt, provider.GetRequiredService<AdataStrategy>() },
        { InvoiceType.Kz, provider.GetRequiredService<PkAdataStrategy>() },
        { InvoiceType.KzAkt, provider.GetRequiredService<PkAdataStrategy>() }
    };

    public ICounterpartyDataStrategy GetDataStrategy(InvoiceType type)
        => _strategies.TryGetValue(type, out var strategy)
            ? strategy
            : throw new NotSupportedException($"Strategy for {type} not found");
}
