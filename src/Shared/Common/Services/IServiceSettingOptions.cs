namespace AppCommonLib.Services
{
    public interface IServiceSettingOptions
    {
        string Audience { get; set; }
        string BaseUrl { get; set; }
        string Scopes { get; set; }
    }
}