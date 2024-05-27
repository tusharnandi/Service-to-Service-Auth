
namespace GlobalView.Services;

public interface ICountryHeadService
{
    Task<List<string>> GetProcurementDetailByCountry(string countryName);
}