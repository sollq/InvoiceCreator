using System.Text.Json;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class InvoiceNumberCounterService(string? filePath = null) : IInvoiceNumberCounterService
{
    private readonly string _filePath =
        filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoice_counters.json");

    private readonly object _lock = new();

    public string SetNextNumber(DocumentType org)
    {
        lock (_lock)
        {
            var data = Load();
            int next;
            switch (org)
            {
                case DocumentType.Ru:
                    data.Ru++;
                    next = data.Ru;
                    break;
                case DocumentType.Kz:
                    data.Kz++;
                    next = data.Kz;
                    break;
                case DocumentType.KzAkt:
                    data.KzAkt++;
                    next = data.KzAkt;
                    break;
                case DocumentType.RuAkt:
                    data.RuAkt++;
                    next = data.RuAkt;
                    break;
                default:
                    throw new ArgumentException("Unknown org type");
            }

            Save(data);
            return $"{next}";
        }
    }

    public string GetNextNumber(DocumentType org)
    {
        var data = Load();
        var next = org switch
        {
            DocumentType.Ru => data.Ru + 1,
            DocumentType.Kz => data.Kz + 1,
            DocumentType.KzAkt => data.KzAkt + 1,
            DocumentType.RuAkt => data.RuAkt + 1,
            _ => throw new ArgumentException("Unknown org type")
        };
        return $"{next}";
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

    private class CounterData
    {
        public int Ru { get; set; }
        public int Kz { get; set; }
        public int KzAkt { get; set; }
        public int RuAkt { get; set; }
    }
}