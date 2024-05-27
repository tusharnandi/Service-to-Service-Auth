using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace fKolkataApi.Services;

[Authorize]
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    [RequiredScope("KolkataFactory.Reader")]
    public async Task<string[]> GetOrders()
    {
        _logger.LogInformation($"Start KolkataApi GetOrders() scope:KolkataFactory.Reader");
        var summaries = new[]
        {
            "Kol-Chilly", "Kol-Warm", "Kol-Balmy", "Kol-Hot"
        };
        
        return await Task.FromResult(summaries);
    }
}
