using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Models;
using Infrastructure.Integrations.Interfaces;

namespace Infrastructure.Integrations;

public class RuStrategy : IPartyInfoStrategy
{
    public bool CanHandle(DocumentType type)
    {
        return type is DocumentType.InvoiceRu or DocumentType.RuAkt;
    }

    public async Task<ClientInfo> GetData(string id)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", "Token 325ebcc995e0d94ce16cd71441de26bd5bdb7561");
        var clientInfo = new ClientInfo();
        var requestBody = JsonSerializer.Serialize(new { query = id.Trim() });
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party",
            content);
        if (!response.IsSuccessStatusCode)
            return clientInfo;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("suggestions", out var suggestions) || suggestions.GetArrayLength() == 0)
        {
            return clientInfo;
        }

        var firstSuggestion = suggestions[0];
        if (!firstSuggestion.TryGetProperty("data", out var data))
        {
            return clientInfo;
        }

        if (data.TryGetProperty("name", out var nameElement) && nameElement.TryGetProperty("short_with_opf", out var nameValue))
        {
            clientInfo.Name = nameValue.GetString() ?? string.Empty;
        }

        if (data.TryGetProperty("address", out var addressElement) && addressElement.TryGetProperty("value", out var addressValue))
        {
            clientInfo.Address = addressValue.GetString() ?? string.Empty;
        }

        if (data.TryGetProperty("kpp", out var kppValue))
        {
            clientInfo.KPP = kppValue.GetString() ?? string.Empty;
        }

        return clientInfo;
    }
}