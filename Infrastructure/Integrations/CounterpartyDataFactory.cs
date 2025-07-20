using Core.Models;
using Infrastructure.Integrations.Interfaces;

namespace Infrastructure.Integrations;

public class CounterpartyDataFactory(IEnumerable<IPartyInfoStrategy> strategies) : ICounterpartyDataFactory
{
    public IPartyInfoStrategy GetDataStrategy(DocumentType type)
    {
        return strategies.FirstOrDefault(s => s.CanHandle(type))
               ?? throw new NotSupportedException("Подходящая для обработки движок - не найдена.");
    }
}