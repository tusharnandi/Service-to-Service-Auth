using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace fDhakaApi.Services;

[Authorize]
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    [RequiredScope("Factory.Reader")]
    public async Task<string[]> GetOrders()
    {
        _logger.LogInformation($"Start DhakaApi GetOrders() scope:Factory.Reader");
        var summaries = new[]
        {
            "Dhaka-Freezing", "Dhaka-Bracing", "Dhaka-Chilly", "Dhaka-Cool"
        };

        return await Task.FromResult(summaries);
    }
}
