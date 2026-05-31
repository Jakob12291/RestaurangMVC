using Microsoft.AspNetCore.Mvc;
using RestaurangMVC.Models;
using RestaurangMVC.Services;

namespace RestaurangMVC.Controllers;

/// <summary>
/// Publik sida där besökare kan söka lediga bord (använder det öppna
/// endpointet /api/bord/lediga). Själva bokningen hanteras av administratörer.
/// </summary>
public class BokningController : Controller
{
    private readonly RestaurantApiClient _api;

    public BokningController(RestaurantApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Index() => View(new LedigaBordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LedigaBordViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            model.Resultat = await _api.GetLedigaBordAsync(model.Datum, model.Tid, model.AntalGaster, ct);
            model.HarSökt = true;
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Kunde inte hämta lediga bord just nu. Försök igen senare.");
        }
        return View(model);
    }
}
