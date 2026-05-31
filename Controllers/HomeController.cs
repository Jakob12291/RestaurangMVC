using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurangMVC.Models;
using RestaurangMVC.Services;

namespace RestaurangMVC.Controllers;

public class HomeController : Controller
{
    private readonly RestaurantApiClient _api;

    public HomeController(RestaurantApiClient api) => _api = api;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var model = new HomeViewModel();
        try
        {
            var meny = await _api.GetMenyAsync(ct);
            model.PopuläraRätter = meny.Where(m => m.IsPopular).Take(3).ToList();
            if (model.PopuläraRätter.Count == 0)
                model.PopuläraRätter = meny.Take(3).ToList();
        }
        catch
        {
            // API ej tillgängligt – startsidan visas ändå utan rätter.
        }
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
