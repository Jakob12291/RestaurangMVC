using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurangMVC.Models;
using RestaurangMVC.Services;

namespace RestaurangMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class MenyController : Controller
{
    private readonly RestaurantApiClient _api;

    public MenyController(RestaurantApiClient api) => _api = api;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _api.GetMenyAsync(ct));

    [HttpGet]
    public IActionResult Create() => View(new MenyFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenyFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        if (await _api.CreateMenyAsync(model, ct))
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, "Kunde inte skapa rätten.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var m = await _api.GetMenyItemAsync(id, ct);
        if (m is null) return NotFound();
        return View(new MenyFormModel
        {
            Id = m.Id,
            Namn = m.Namn,
            Pris = m.Pris,
            Beskrivning = m.Beskrivning,
            IsPopular = m.IsPopular,
            BildUrl = m.BildUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MenyFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        if (await _api.UpdateMenyAsync(model, ct))
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, "Kunde inte uppdatera rätten.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var m = await _api.GetMenyItemAsync(id, ct);
        if (m is null) return NotFound();
        return View(m);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _api.DeleteMenyAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
