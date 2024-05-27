using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Abstractions;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace GlobalView.Services;

[Authorize]
public class CountryHeadService : ICountryHeadService
{
    private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;
    private List<MyEndpoint> endpoints = new List<MyEndpoint>();

    
    
    public CountryHeadService(IAuthorizationHeaderProvider authorizationHeaderProvider, IConfiguration configuration)
    {
        _authorizationHeaderProvider = authorizationHeaderProvider;

        //endpoints for countries
        endpoints.Add(new MyEndpoint("India", configuration.GetSection("DownstreamApiForCountryHeadIndia")));
        endpoints.Add(new MyEndpoint("Bengladesh", configuration.GetSection("DownstreamApiForCountryHeadBangladesh")));

    }
    public async Task<List<string>> GetProcurementDetailByCountry(string countryName)
    {
        List<string> list = null!;
        try
        {
            var endpoint= endpoints.Find(c=>c.CountryName==countryName);
            if (endpoint!=null)
            {
                var client= await this.HttpClientFactoryAsync(endpoint);

                string content = await client.GetStringAsync("/api/Factories/list/all");

                var result = JsonSerializer.Deserialize<string[]>(content);
                if(result!=null)
                    list = result.ToList();

            }
            else
            {
                throw new ApplicationException($"Endpoint is not configured for the country {countryName}");
            }
            
        }
        catch (Exception ex)
        {
            list= new List<string> { $"Error on countryHeadService {countryName}: {ex.Message}" };
        }
        
        return list;
    }

    private async Task<HttpClient> HttpClientFactoryAsync(MyEndpoint endpoint)
    {
        var client = new HttpClient();
        string accessToken = await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(endpoint.Scopes);

        client.BaseAddress = new Uri(endpoint.BaseUrl);
        client.DefaultRequestHeaders.Add("Authorization", accessToken);
        return client;
    }
}

public class MyEndpoint
{
    public string BaseUrl { get; set; }
    public string CountryName { get; set; }
    public string[] Scopes { get; set; }
    public MyEndpoint(string factory, IConfigurationSection configurationSection)
    {
        this.CountryName = factory;
        this.BaseUrl = configurationSection.GetValue<string>("BaseUrl") ?? "";

        string scope = configurationSection.GetValue<string>("Scopes") ?? "";
        string audience = configurationSection.GetValue<string>("Audience") ?? "";
        if (scope != "" && audience != "")
        {
            this.Scopes = new string[] { $"{audience}/{scope}" };
        }
        else
        {
            this.Scopes = new string[] { };
        }

    }


}

