using System.Text.Json;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class InvoiceNumberCounterService(string? filePath = null) : IInvoiceNumberCounterService
{
    private readonly string _filePath =
        filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoice_counters.json");

    private readonly object _lock = new();

    public string SetNumber(DocumentType org, string inputInvoiceNumber)
    {
        var tryParse = int.TryParse(inputInvoiceNumber, out var number);
        if (!tryParse)
        {
            throw new ArgumentException("Некорректный номер счета");
        }
        lock (_lock)
        {
            var data = Load();
            int next;
            switch (org)
            {
                case DocumentType.InvoiceRu:
                    data.Ru = number;
                    next = data.Ru;
                    break;
                case DocumentType.InvoiceKz:
                    data.Kz = number;
                    next = data.Kz;
                    break;
                case DocumentType.KzAkt:
                    data.KzAkt = number;
                    next = data.KzAkt;
                    break;
                case DocumentType.RuAkt:
                    data.RuAkt = number;
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
            DocumentType.InvoiceRu => data.Ru,
            DocumentType.InvoiceKz => data.Kz,
            DocumentType.KzAkt => data.KzAkt,
            DocumentType.RuAkt => data.RuAkt,
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