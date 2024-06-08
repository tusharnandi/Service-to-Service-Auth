namespace AppCommonLib.Services;

public class ServiceEndpoint
{
    public string BaseUrl { get; set; }
    public string ServiceName { get; set; }
    public string[] Scopes { get; set; }
    public ServiceEndpoint(string serviceName, IServiceSettingOptions? serviceSetting)
    {
        ServiceName = serviceName;
        BaseUrl = (serviceSetting!=null) ? serviceSetting.BaseUrl : "";

        string scope = (serviceSetting != null) ? serviceSetting.Scopes : "";
        string audience = (serviceSetting != null) ? serviceSetting.Audience: "";
        if (scope != "" && audience != "")
        {
            Scopes = new string[] { $"{audience}/{scope}" };
        }
        else
        {
            Scopes = new string[] { };
        }

    }


}
