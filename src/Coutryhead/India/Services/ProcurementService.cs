using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web.Resource;
using System.Text.Json;

namespace cInApi.Services;

[Authorize]
public class ProcurementService : IProcurementService
{
    private List<MyEndpoint> endpoints = new List<MyEndpoint>();
    private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;
    private readonly ILogger<ProcurementService> _logger;
    private readonly IConfiguration _configuration;

    public ProcurementService(IAuthorizationHeaderProvider authorizationHeaderProvider, IConfiguration configuration, ILogger<ProcurementService> logger)
    {
        _authorizationHeaderProvider = authorizationHeaderProvider;
        _logger = logger;
        _configuration = configuration;

        //endpoints for factories
        endpoints.Add(new MyEndpoint("Bangalore",  _configuration.GetSection("DownstreamApiForIndiaBangalore")));
        endpoints.Add(new MyEndpoint("Hyderabad", _configuration.GetSection("DownstreamApiForIndiaHyderabad")));
        endpoints.Add(new MyEndpoint("Kolkata", _configuration.GetSection("DownstreamApiForIndiaKolkata")));

        //Wrong Configuration Testing - it should be failed because Audience mis-match:
        //endpoints.Add(new MyEndpoint("Dhaka", _configuration.GetSection("DownstreamApiForBangladeshDhaka")));
    }

    [RequiredScope("India.Reader")]
    public async Task<List<string>> GetProcurementsByFactory(string factory)
    {
        List<string> list = null!;
        _logger.LogInformation($"Start GetProcurementsByFactory({factory}) scope:India.Reader");
        try
        {
            var endpoint = endpoints.Find(c => c.FactoryName == factory);
            if (endpoint != null)
            {
                var client = await this.HttpClientFactoryAsync(endpoint);

                string content = await client.GetStringAsync("/api/Orders/list");

                var result = JsonSerializer.Deserialize<string[]>(content);
                if (result != null)
                    list = result.ToList();

            }
            else
            {
                throw new ApplicationException($"Endpoint is not configured for the factory {factory}");
            }

        }
        catch (Exception ex)
        {
            list = new List<string> { $"Error on factoryService {factory}: {ex.Message}" };
        }
        finally
        {
            _logger.LogInformation($"End GetProcurementsByFactory({factory}) scope:India.Reader");
        }

        return list;
    }


    public async Task<string[]> GetAllFactories()
    {
        List<string> list = new List<string>();
        foreach (var endpoint in endpoints)
        {
            list.Add(endpoint.FactoryName);
        }

        return await Task.FromResult(list.ToArray());
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
    public string FactoryName { get; set; }
    public string[] Scopes { get; set; }

    public MyEndpoint(string factory, IConfigurationSection configurationSection)
    {
        this.FactoryName = factory;
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
