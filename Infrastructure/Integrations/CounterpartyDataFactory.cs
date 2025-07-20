using Core.Models;
using Infrastructure.Integrations.Interfaces;
using Infrastructure.Pdf;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF;

namespace Infrastructure.Integrations;

public class CounterpartyDataFactory(IEnumerable<IPartyInfoStrategy> strategies) : ICounterpartyDataFactory
{
    public IPartyInfoStrategy GetDataStrategy(InvoiceType type)
    {
        return strategies.FirstOrDefault(s => s.CanHandle(type))
               ?? throw new NotSupportedException($"Подходящая для обработки движок - не найдена.");
    }
}

