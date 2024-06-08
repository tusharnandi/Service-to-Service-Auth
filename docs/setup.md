

># STS: 'Service-to-Service' Calls on behalf of user.
>It also knows as WebAPI That calls another Web API on behalf of user.

Your app needs to acquire a token for the downstram Web API(called A). That  Web API(A) again needs to calls another downstram Web API(called B).


![High level diagram](./images/service-2-service-v1.png)


Plese find the video for demo


>dotnet new sln -n Global

## Global 
>dotnet new mvc -o .\src\Global\GlobalView -n GlobalView
>dotnet sln add .\src\Global\GlobalView\GlobalView.csproj

### Coutry Head:
> dotnet new webapi -o .\src\Coutryhead\India -n cInApi
> dotnet sln add .\src\Coutryhead\India\cInApi.csproj

> dotnet new webapi -o .\src\Coutryhead\Bangladesh -n cBaApi
> dotnet sln add .\src\Coutryhead\Bangladesh\cBaApi.csproj

### Factories:
>  dotnet new webapi -o .\src\Factories\Bangladesh\Daka -n fDhakaApi
>  dotnet sln add .\src\Factories\Bangladesh\Daka\fDhakaApi.csproj


>  dotnet new webapi -o .\src\Factories\India\Bangalore -n fBangaloreApi
>  dotnet sln add .\src\Factories\India\Bangalore\fBangaloreApi.csproj

>  dotnet new webapi -o .\src\Factories\India\Kolkata -n fKolkataApi
>  dotnet sln add .\src\Factories\India\Kolkata\fKolkataApi.csproj
>
> 
>  dotnet new webapi -o .\src\Factories\India\Hyderabad -n fHyderabadApi
>  dotnet sln add .\src\Factories\India\Hyderabad\fHyderabadApi.csproj


>  dotnet new classlib -o .\src\Shared\Cache -n RadisCacheLib
>  dotnet sln add .\src\Shared\Cache\RadisCacheLib.csproj

>  dotnet new classlib -o .\src\Shared\Common -n AppCommonLib
>  dotnet sln add .\src\Shared\Common\AppCommonLib.csproj
### App Registraions



**1. Register Factories**

>**Kolkata Factory Registration Steps:**
>
>App Name: API-India-Kolkata-Factory
>
>**Expose an API:**  https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-965b-4b0e-897c-211134f25c16
>   - **Add a scope:** 
>       - Scope name: KolkataFactory.Reader
>       - Admin consent display name: Kolkata Factory Reader
>       - Admin consent description: Allows the app users to read Kolkata factory related information
>
>   - **Manifest:** "accessTokenAcceptedVersion": 2
>
>
> Do the same for Indian's 'Hyderabad', 'Bangalore' as well.
> Also do the same for Bengladesh's 'Dhaka'


**2. Register Country Heads**
> **India** 
> 
> App Name: API-India-Country-Head
> (Single tenant)
> 
> **Expose an API:** https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-44fa-49be-9e87-2bb6f290511a
>
> **Manifest:** "accessTokenAcceptedVersion": 2
> **Certificates & secrets:**
>    - **ClientSecret:** 
> - **API permissions:**
>    - 1) https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-4ebc-45c7-a177-8101ee0deee6/Factory.Reader  (Grant Admin Consent)
>    - 2) https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-685d-4c4f-8888-0091ffe474da/HyderabadFactory.Reader  (Grant Admin Consent)
>    - 3) https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-965b-4b0e-897c-211134f25c16/KolkataFactory.Reader (Grant Admin Consent)
>
> **Bengladesh** (Do the same) 
>
>App Name: API-Bangladesh-Country-Head
>
> (Single tenant)
> 
> **Expose an API:** https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-55ee-49b2-81bf-13b0e704be0a
> 
> **Manifest:** "accessTokenAcceptedVersion": 2
> **Certificates & secrets:**
>    - **ClientSecret:** 
> - **API permissions:**
>    - 1) https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-35ee-4089-9e2e-56974b5682b0/Factory.Reader  (Grant Admin Consent)


**1. Main Web App**

>App Name: APP-GlobalView
>
>Platfom Web:
>    - Redirect URIs: https://localhost:5010/signin-oidc
>    - Front-channel logout URL: https://localhost:5010/signout-oidc
>    - ClientSecret:
>
>    - API permissions: 
>    - 1) https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-44fa-49be-9e87-2bb6f290511a/India.Reader  (Grant Admin Consent)
>    - 2) https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-55ee-49b2-81bf-13b0e704be0a/Bangladesh.Reader  (Grant Admin Consent)
>
> URL: https://localhost:5010/


## Code Configuration

>**Kolkata Factory:**
>
>appsettings.json
>```
>{
>  "MicrosoftEntraId": {
>    "Instance": "https://login.microsoftonline.com/",
>    "ClientId": "xxxxxxxxx-965b-4b0e-897c-211134f25c16", //API-India-Kolkata-Factory
>    "TenantId": "xxxxxxxxx-50ab-4e18-8064-cefe710e2c6d"
>  },
>}
>```
>
>Startup.cs
>```
>services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
>    .AddMicrosoftIdentityWebApi(_configuration.GetSection("MicrosoftEntraId"));
>
>```
>
>OrdersController.cs
>```
>[Authorize]
>public class OrdersController : ControllerBase
>{
>    [RequiredScope("KolkataFactory.Reader")]
>    public async Task<List<string>> GetAllProcurements()
>    {
>
>    }
>}
>```
>
### Country Head India 

appsettings.json
```
{
  "MicrosoftEntraId": {
    "Instance": "https://login.microsoftonline.com/",
    "ClientId": "xxxxxxxxx-44fa-49be-9e87-2bb6f290511a", //API-India-Country-Head
    "TenantId": "xxxxxxxxx-50ab-4e18-8064-cefe710e2c6d",
    "ClientSecret": ""
  },

  "DownstreamApiForIndiaKolkata": {
    "BaseUrl": "https://localhost:8001", //Kolkata Factory
    "Scopes": "KolkataFactory.Reader"
  },
  "DownstreamApiForIndiaHyderabad": {
    "BaseUrl": "https://localhost:7001", //Hyderabad Factory
    "Scopes": "HyderabadFactory.Reader"
  },
  "DownstreamApiForIndiaBangalore": {
    "BaseUrl": "https://localhost:6001", //Bangalore Factory
    "Scopes": "Factory.Reader"
  }
}

```

Startup.cs
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(_configuration.GetSection("MicrosoftEntraId"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddDownstreamApi("India-Bangalore-Factory", _configuration.GetSection("DownstreamApiForIndiaBangalore"))
            .AddDownstreamApi("India-Hyderabad-Factory", _configuration.GetSection("DownstreamApiForIndiaHyderabad "))
            .AddDownstreamApi("India-Kolkata-Factory", _configuration.GetSection("DownstreamApiForIndiaKolkata"))
            .AddInMemoryTokenCaches();
}
```
FactoriesController.cs

```
[Authorize]
public class FactoriesController : ControllerBase
{
    [RequiredScope("India.Reader")]
    public async Task<List<string>> GetAllProcurements()
    {
        
        endpoint =new MyEndpoint("Kolkata",    "https://localhost:8001", "https://mytestdirectory.onmicrosoft.com/xxxxxxxxx-965b-4b0e-897c-211134f25c16/KolkataFactory.Reader");

        var client = await this.HttpClientFactoryAsync(endpoint);

        string content = await client.GetStringAsync("/api/Orders/list");

        var result = JsonSerializer.Deserialize<string[]>(content);
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

    public MyEndpoint(string factory, string url, string scope)
    {
        this.FactoryName = factory;
        this.BaseUrl = url;
        this.Scopes = new string[] { scope };
    }

}

```