using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurangMVC.Models;
using RestaurangMVC.Services;

namespace RestaurangMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BordController : Controller
{
    private readonly RestaurantApiClient _api;

    public BordController(RestaurantApiClient api) => _api = api;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _api.GetBordAsync(ct));

    [HttpGet]
    public IActionResult Create() => View(new BordFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BordFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        if (await _api.CreateBordAsync(model, ct))
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, "Kunde inte skapa bordet (bordsnummer kan redan finnas).");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var b = await _api.GetBordItemAsync(id, ct);
        if (b is null) return NotFound();
        return View(new BordFormModel { Id = b.Id, Bordsnummer = b.Bordsnummer, Kapacitet = b.Kapacitet });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BordFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        if (await _api.UpdateBordAsync(model, ct))
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, "Kunde inte uppdatera bordet.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var b = await _api.GetBordItemAsync(id, ct);
        if (b is null) return NotFound();
        return View(b);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _api.DeleteBordAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
