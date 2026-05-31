using Microsoft.AspNetCore.Mvc;
using RestaurangMVC.Models;
using RestaurangMVC.Services;

namespace RestaurangMVC.Controllers;

public class MenyController : Controller
{
    private readonly RestaurantApiClient _api;

    public MenyController(RestaurantApiClient api) => _api = api;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        List<MenyItem> meny;
        try
        {
            meny = await _api.GetMenyAsync(ct);
        }
        catch
        {
            ViewBag.Fel = "Kunde inte hämta menyn just nu. Försök igen senare.";
            meny = new();
        }
        return View(meny);
    }
}
