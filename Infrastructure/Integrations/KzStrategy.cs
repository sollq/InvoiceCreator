using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Models;
using Infrastructure.Integrations.Interfaces;

namespace Infrastructure.Integrations;

public class KzStrategy : IPartyInfoStrategy
{
    public async Task<ClientInfo> GetData(string bin)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", "Token 325ebcc995e0d94ce16cd71441de26bd5bdb7561");
        var clientInfo = new ClientInfo();
        var requestBody = JsonSerializer.Serialize(new { query = bin.Trim() });
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party_kz",
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

        if (data.TryGetProperty("name_ru", out var nameValue))
        {
            clientInfo.Name = nameValue.GetString() ?? string.Empty;
        }

        if (data.TryGetProperty("address_ru", out var addressValue))
        {
            clientInfo.Address = addressValue.GetString() ?? string.Empty;
        }

        return clientInfo;
    }

    public bool CanHandle(DocumentType type)
    {
        return type is DocumentType.Kz or DocumentType.KzAkt;
    }
}