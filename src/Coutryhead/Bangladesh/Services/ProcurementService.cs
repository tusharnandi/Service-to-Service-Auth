using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web.Resource;
using System.Text.Json;
using RadisCacheLib.DistrubutedCacheHelper;
using AppCommonLib.Services;
namespace cBaApi.Services;

[Authorize]
public class ProcurementService : IProcurementService
{
    
    private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;
    private readonly ILogger<ProcurementService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache _resourceCache;
    private readonly IServiceSettingOptions _dhakaServiceSettings;
    //private readonly IServiceSettingOptions _kolkataServiceSettings;
    private List<ServiceEndpoint> endpoints = new List<ServiceEndpoint>();

    public ProcurementService(IAuthorizationHeaderProvider authorizationHeaderProvider,IConfiguration configuration, IDistributedCache resourceCache, ILogger<ProcurementService> logger)
    {
        _authorizationHeaderProvider = authorizationHeaderProvider;
        _logger = logger;
        _configuration = configuration;
        _resourceCache = resourceCache;

        //endpoints for factories
        _dhakaServiceSettings = new ServiceSettingOptions();
        _configuration.GetSection("DownstreamApiForBangladeshDhaka").Bind(_dhakaServiceSettings);
        endpoints.Add(new ServiceEndpoint("Dhaka", _dhakaServiceSettings));

        //Wrong Configuration Testing - it should be failed because Audience mis-match:
        //_kolkataServiceSettings = new ServiceSettingOptions();
        //_configuration.GetSection("DownstreamApiForIndiaKolkata").Bind(_kolkataServiceSettings);
        //endpoints.Add(new ServiceEndpoint("Kolkata", _kolkataServiceSettings));
    }

    [RequiredScope("Bangladesh.Reader")]
    public async Task<List<string>> GetProcurementsByFactory(string factory)
    {
        List<string> list = null!;
        string recordKey = $"cd:bangladesh:{factory}";
        _logger.LogInformation($"Start GetProcurementsByFactory({factory}) scope:Bangladesh.Reader");
        try
        {
            var record = await _resourceCache.GetRecordAsync<List<string>>(recordKey);
            if (record == null)
            {
                var endpoint = endpoints.Find(c => c.ServiceName == factory);
                if (endpoint != null)
                {
                    var client = await this.HttpClientFactoryAsync(endpoint);

                    string content = await client.GetStringAsync("/api/Orders/list");

                    var result = JsonSerializer.Deserialize<string[]>(content);
                    if (result != null)
                    {
                        list = result.ToList();
                        TimeSpan absoluteExpireTime = TimeSpan.FromMinutes(30);
                        TimeSpan slidingExpireTime = TimeSpan.FromMinutes(20);
                        await _resourceCache.SetRecordAsync(recordKey, list, absoluteExpireTime,slidingExpireTime); // Set cache
                    }


                }
                else
                {
                    throw new ApplicationException($"Endpoint is not configured for the factory {factory}");
                }
            }
            else
            {
                list = record;
            }
                

        }
        catch (Exception ex)
        {
            list = new List<string> { $"Error on factoryService {factory}: {ex.Message}" };
        }
        finally
        {
            _logger.LogInformation($"End GetProcurementsByFactory({factory}) scope:Bangladesh.Reader");
        }

        

        return list;
    }


    public async Task<string[]> GetAllFactories()
    {
        List<string> list = new List<string>();
        foreach (var endpoint in endpoints)
        {
            list.Add(endpoint.ServiceName);
        }

        return await Task.FromResult(list.ToArray());
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