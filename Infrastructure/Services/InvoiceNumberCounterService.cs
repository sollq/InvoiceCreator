using System.Text.Json;
using Core.Models;
using Core.Interfaces;

namespace Infrastructure.Services;

public class InvoiceNumberCounterService(string? filePath = null) : IInvoiceNumberCounterService
{
    private readonly string _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoice_counters.json");
    private readonly object _lock = new();

    private class CounterData
    {
        public int Ru { get; set; }
        public int Kz { get; set; }
    }

    public string GetNextNumber(OrganizationType org)
    {
        lock (_lock)
        {
            var data = Load();
            int next;
            switch (org)
            {
                case OrganizationType.Ru:
                    data.Ru++;
                    next = data.Ru;
                    break;
                case OrganizationType.Kz:
                    data.Kz++;
                    next = data.Kz;
                    break;
                default:
                    throw new ArgumentException("Unknown org type");
            }
            Save(data);
            return $"СЧЕТ-{next:D5}";
        }
    }

    public string PeekNextNumber(OrganizationType org)
    {
        var data = Load();
        int next = org switch
        {
            OrganizationType.Ru => data.Ru + 1,
            OrganizationType.Kz => data.Kz + 1,
            _ => throw new ArgumentException("Unknown org type")
        };
        return $"СЧЕТ-{next:D5}";
    }

    private CounterData Load()
    {
        if (!File.Exists(_filePath))
            return new CounterData();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<CounterData>(json) ?? new CounterData();
    }

    private void Save(CounterData data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
} 