using GlobalView.Models;
using GlobalView.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GlobalView.Controllers;

[Authorize]
public class ProcurementController : Controller
{
    private readonly ICountryHeadService _countryHeadService;

    public ProcurementController(ICountryHeadService countryHeadService)
    {
        _countryHeadService = countryHeadService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(string button)
    {
        string countryName = button;

        List<string> result = await _countryHeadService.GetProcurementDetailByCountry(countryName);

        ItemViewModel itemView = new ItemViewModel();

        itemView.items = result;

        Console.WriteLine($"\nDisplaying Response from '{countryName} Service' ...");
        foreach (var item in result)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine($"End Response from '{countryName} Service'.");

        //ViewData["result"] = result;
        return View(itemView);
    }

}
