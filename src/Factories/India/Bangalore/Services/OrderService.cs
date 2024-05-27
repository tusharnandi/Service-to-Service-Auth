using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace fBangaloreApi.Services;

[Authorize]
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService>  logger)
    {
        _logger = logger;
    }

    [RequiredScope("Factory.Reader")]
    public async Task<string[]> GetOrders()
    {
        _logger.LogInformation($"Start BangaloreApi GetOrders() scope:Factory.Reader");
        var summaries = new[]
        {
            "Bglr-Freezing", "Bglr-Bracing", "Bglr-Chilly", "Bglr-Cool"
        };

        return await Task.FromResult(summaries);
    }
}
