using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Abstractions;
using System.Net.Http.Headers;
using System.Text.Json;
using AppCommonLib.Services;
using System.Collections.Generic;

namespace GlobalView.Services;

[Authorize]
public class CountryHeadService : ICountryHeadService
{
    private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;
    private readonly IConfiguration _configuration;
    private readonly IServiceSettingOptions _indiaServiceSettings;
    private readonly IServiceSettingOptions _bangladeshServiceSettings;
    private readonly List<ServiceEndpoint> endpoints = new List<ServiceEndpoint>();

    
    
    public CountryHeadService(IAuthorizationHeaderProvider authorizationHeaderProvider, IConfiguration configuration)
    {
        _authorizationHeaderProvider = authorizationHeaderProvider;
        _configuration= configuration;

        _indiaServiceSettings = new ServiceSettingOptions();
        _bangladeshServiceSettings = new ServiceSettingOptions();

        _configuration.GetSection(ServiceSettingOptions.ServiceSettingsForCountryIndia).Bind(_indiaServiceSettings);
        _configuration.GetSection(ServiceSettingOptions.ServiceSettingsForCountryBangladesh).Bind(_bangladeshServiceSettings);

        //endpoints for countries
        endpoints.Add(new ServiceEndpoint("India", _indiaServiceSettings));
        endpoints.Add(new ServiceEndpoint("Bengladesh", _bangladeshServiceSettings));
    }
    public async Task<List<string>> GetProcurementDetailByCountry(string countryName)
    {
        List<string> list = null!;
        try
        {
            var endpoint= endpoints.Find(c=>c.ServiceName==countryName);
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

    private async Task<HttpClient> HttpClientFactoryAsync(ServiceEndpoint endpoint)
    {
        var client = new HttpClient();
        string accessToken = await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(endpoint.Scopes);

        client.BaseAddress = new Uri(endpoint.BaseUrl);
        client.DefaultRequestHeaders.Add("Authorization", accessToken);
        return client;
    }
}


