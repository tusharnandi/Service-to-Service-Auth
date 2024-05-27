
namespace cBaApi.Services
{
    public interface IProcurementService
    {
        Task<string[]> GetAllFactories();
        Task<List<string>> GetProcurementsByFactory(string factory);
    }
}