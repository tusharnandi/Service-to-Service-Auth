using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace fHyderabadApi.Services;

[Authorize]
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    [RequiredScope("HyderabadFactory.Reader")]
    public async Task<string[]> GetOrders()
    {
        _logger.LogInformation($"Start HyderabadApi GetOrders() scope:HyderabadFactory.Reader");

        var summaries = new[]
        {
            "Hd-Bracing", "Hd-Chilly", "Hd-Scorching"
        };

        return await Task.FromResult(summaries);
    }
}