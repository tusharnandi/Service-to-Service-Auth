using fBangaloreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace fBangaloreApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpGet("list")]
    [RequiredScope("Factory.Reader")]
    public async Task<List<string>> GetAllProcurements()
    {
        return (await _orderService.GetOrders()).ToList();
    }
}
