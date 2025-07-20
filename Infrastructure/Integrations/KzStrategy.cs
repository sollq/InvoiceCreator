using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Core.Models;
using Infrastructure.Integrations.Interfaces;
using System.Linq;
using System;
using Dadata;
using Dadata.Model;

namespace Infrastructure.Integrations;

public class KzStrategy : IPartyInfoStrategy
{
    public async Task<ClientInfo> GetData(string bin)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", "Token 325ebcc995e0d94ce16cd71441de26bd5bdb7561");
        var clientInfo = new ClientInfo();
        var requestBody = JsonSerializer.Serialize(new { query = bin });
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party_kz", content);
        if (!response.IsSuccessStatusCode)
            return clientInfo;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var suggestions = doc.RootElement.GetProperty("suggestions");
        if (suggestions.GetArrayLength() == 0)
            return clientInfo;

        var data = suggestions[0].GetProperty("data");

        clientInfo.Name = data.GetProperty("name_ru").GetString();
        clientInfo.Address = data.GetProperty("address_ru").GetString();

        return clientInfo;
    }
    public bool CanHandle(InvoiceType type)
    {
        return type is InvoiceType.Kz or InvoiceType.KzAkt;
    }
}