﻿using cBaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace cBaApi;

public class Startup
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //Cache
        if (!string.IsNullOrEmpty(_configuration.GetConnectionString("RedisConStr")))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _configuration.GetConnectionString("RedisConStr");
                options.InstanceName = "BangladeshHD";
            });
        }
        //Cache

        /*
     'Scopes' contains space separated scopes of the web API you want to call. This can be:
      - a scope for a V2 application (for instance api://b3682cc7-8b30-4bd2-aaba-080c6bf0fd31/access_as_user)
      - a scope corresponding to a V1 application (for instance <App ID URI>/.default, where  <App ID URI> is the
        App ID URI of a legacy v1 web application
      Applications are registered in the https://portal.azure.com portal.
    */
        //EntraId
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(_configuration.GetSection("MicrosoftEntraId"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddDownstreamApi("Bangladesh-Dhaka-Factory", _configuration.GetSection("DownstreamApiForBangladeshDhaka"))
                .AddDownstreamApi("India-Kolkata-Factory", _configuration.GetSection("DownstreamApiForIndiaKolkata")) //Wrong Configuration testing
                .AddInMemoryTokenCaches();
        //EntraId


        services.AddControllers();

        services.AddCors(options =>
        {
            // this defines a CORS policy called "default"
            options.AddPolicy("default", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });


        services.AddScoped<IProcurementService, ProcurementService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

    }

    public void Configure(IApplicationBuilder app)
    {
        if (_environment.IsDevelopment())
        {
            //IdentityModelEventSource.ShowPII = true;

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseRouting();

        app.UseCors("default");

        //EntraId
        app.UseAuthentication();
        app.UseAuthorization();
        //EntraId

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}