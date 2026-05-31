using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurangMVC.Models;
using RestaurangMVC.Services;

namespace RestaurangMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BokningarController : Controller
{
    private readonly RestaurantApiClient _api;

    public BokningarController(RestaurantApiClient api) => _api = api;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _api.GetBokningarAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new BokningFormModel { TillgängligaBord = await _api.GetBordAsync(ct) };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BokningFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.TillgängligaBord = await _api.GetBordAsync(ct);
            return View(model);
        }

        var (ok, error) = await _api.CreateBokningAsync(model, ct);
        if (ok) return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, error ?? "Kunde inte skapa bokningen.");
        model.TillgängligaBord = await _api.GetBordAsync(ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var b = await _api.GetBokningAsync(id, ct);
        if (b is null) return NotFound();
        var model = new BokningFormModel
        {
            Id = b.Id,
            BordId = b.BordId,
            StartTid = b.StartTid,
            AntalGaster = b.AntalGaster,
            KundNamn = b.KundNamn,
            KundTelefon = "-",
            TillgängligaBord = await _api.GetBordAsync(ct)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BokningFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.TillgängligaBord = await _api.GetBordAsync(ct);
            return View(model);
        }

        var (ok, error) = await _api.UpdateBokningAsync(model, ct);
        if (ok) return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, error ?? "Kunde inte uppdatera bokningen.");
        model.TillgängligaBord = await _api.GetBordAsync(ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var b = await _api.GetBokningAsync(id, ct);
        if (b is null) return NotFound();
        return View(b);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _api.DeleteBokningAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
