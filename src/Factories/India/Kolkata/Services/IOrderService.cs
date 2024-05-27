
namespace fKolkataApi.Services
{
    public interface IOrderService
    {
        Task<string[]> GetOrders();
    }
}