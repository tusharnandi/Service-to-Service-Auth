namespace AppCommonLib.Services;

public class ServiceSettingOptions : IServiceSettingOptions
{
    public const string ServiceSettingsForCountryIndia = "DownstreamApiForCountryHeadIndia";
    public const string ServiceSettingsForCountryBangladesh = "DownstreamApiForCountryHeadBangladesh";

    public string BaseUrl { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
