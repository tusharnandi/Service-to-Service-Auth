using cInApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Collections.Generic;
using System.Text.Json;

namespace cInApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FactoriesController : ControllerBase
{
    private readonly IProcurementService _procurementService;

    

    public FactoriesController(IProcurementService procurementService)
    {
        _procurementService = procurementService;
    }
    
    
    [HttpGet("list/all")]
    [RequiredScope("India.Reader")]
    public async Task<List<string>> GetAllProcurements()
    {
        List<string> list = new List<string>();
        var factories = await _procurementService.GetAllFactories();

        foreach (var factory in factories)
        {
            var results = await _procurementService.GetProcurementsByFactory(factory);

            if (results != null)
            {
                list.AddRange(results);
            }
        }

        return list;

    }
}




