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
        return type is DocumentType.Ru or DocumentType.RuAkt;
    }

    public async Task<ClientInfo> GetData(string id)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", "Token 325ebcc995e0d94ce16cd71441de26bd5bdb7561");
        var clientInfo = new ClientInfo();
        var requestBody = JsonSerializer.Serialize(new { query = id });
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party",
            content);
        if (!response.IsSuccessStatusCode)
            return clientInfo;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var suggestions = doc.RootElement.GetProperty("suggestions");
        if (suggestions.GetArrayLength() == 0)
            return clientInfo;

        var data = suggestions[0].GetProperty("data");

        clientInfo.Name = data.GetProperty("name").GetProperty("short_with_opf").GetString();
        clientInfo.Address = data.GetProperty("address").GetProperty("value").GetString();

        return clientInfo;
    }
}