
namespace cInApi.Services
{
    public interface IProcurementService
    {

        Task<List<string>> GetProcurementsByFactory(string factory);
        Task<string[]> GetAllFactories();
    }
}