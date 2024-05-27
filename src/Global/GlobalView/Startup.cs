using GlobalView.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace GlobalView;

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
        //EntraId

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(_configuration, "MicrosoftEntraId")
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddDownstreamApi("CountryHead-India", _configuration.GetSection("DownstreamApiForCountryHeadIndia"))
                .AddDownstreamApi("CountryHead-Bangladesh", _configuration.GetSection("DownstreamApiForCountryHeadBangladesh"))
                .AddInMemoryTokenCaches();
        //EntraId

        // Add services to the container.
        services.AddControllersWithViews().AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                          .RequireAuthenticatedUser()
                          .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }).AddMicrosoftIdentityUI();

        services.AddScoped<ICountryHeadService, CountryHeadService>();


    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        if (!_environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        //EntraId
        app.UseAuthentication();
        app.UseAuthorization();
        //EntraId

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
