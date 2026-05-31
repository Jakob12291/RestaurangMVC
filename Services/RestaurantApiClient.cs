using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using RestaurangMVC.Models;

namespace RestaurangMVC.Services;

/// <summary>
/// Klient som kapslar in all kommunikation med REST-API:et. Skickar automatiskt med
/// den inloggade administratörens JWT (lagrad som claim i auth-cookien) vid skyddade anrop.
/// </summary>
public class RestaurantApiClient
{
    public const string JwtClaimType = "api_jwt";

    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public RestaurantApiClient(HttpClient http, IHttpContextAccessor ctx)
    {
        _http = http;
        _ctx = ctx;
    }

    private void AttachToken()
    {
        var token = _ctx.HttpContext?.User?.FindFirst(JwtClaimType)?.Value;
        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    // ---------- Auth ----------
    public async Task<LoginResult?> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/login", new { username, password }, ct);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<LoginResult>(JsonOpts, ct);
    }

    // ---------- Meny (publikt: GET) ----------
    public async Task<List<MenyItem>> GetMenyAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<List<MenyItem>>("api/meny", JsonOpts, ct) ?? new();

    public async Task<MenyItem?> GetMenyItemAsync(int id, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<MenyItem>($"api/meny/{id}", JsonOpts, ct);

    public async Task<bool> CreateMenyAsync(MenyFormModel m, CancellationToken ct = default)
    {
        AttachToken();
        var resp = await _http.PostAsJsonAsync("api/meny", ToMenyPayload(m), ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateMenyAsync(MenyFormModel m, CancellationToken ct = default)
    {
        AttachToken();
        var resp = await _http.PutAsJsonAsync($"api/meny/{m.Id}", ToMenyPayload(m), ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMenyAsync(int id, CancellationToken ct = default)
    {
        AttachToken();
        var resp = await _http.DeleteAsync($"api/meny/{id}", ct);
        return resp.IsSuccessStatusCode;
    }

    private static object ToMenyPayload(MenyFormModel m) => new
    {
        namn = m.Namn,
        pris = m.Pris,
        beskrivning = m.Beskrivning,
        isPopular = m.IsPopular,
        bildUrl = m.BildUrl
    };

    // ---------- Bord ----------
    public async Task<List<Bord>> GetBordAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<List<Bord>>("api/bord", JsonOpts, ct) ?? new();

    public async Task<Bord?> GetBordItemAsync(int id, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<Bord>($"api/bord/{id}", JsonOpts, ct);

    public async Task<List<Bord>> GetLedigaBordAsync(DateTime datum, TimeSpan tid, int antalGaster, CancellationToken ct = default)
    {
        var d = datum.ToString("yyyy-MM-dd");
        var t = tid.ToString(@"hh\:mm");
        return await _http.GetFromJsonAsync<List<Bord>>(
            $"api/bord/lediga?datum={d}&tid={t}&antalGaster={antalGaster}", JsonOpts, ct) ?? new();
    }

    public async Task<bool> CreateBordAsync(BordFormModel b, CancellationToken ct = default)
    {
        AttachToken();
        var resp = await _http.PostAsJsonAsync("api/bord", new { bordsnummer = b.Bordsnummer, kapacitet = b.Kapacitet }, ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBordAsync(BordFormModel b, CancellationToken ct = default)
    {
        AttachToken();
        var resp = await _http.PutAsJsonAsync($"api/bord/{b.Id}", new { bordsnummer = b.Bordsnummer, kapacitet = b.Kapacitet }, ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBordAsync(int id, CancellationToken ct = default)
    {
        AttachToken();
        var resp = await _http.DeleteAsync($"api/bord/{id}", ct);
        return resp.IsSuccessStatusCode;
    }

    // ---------- Bokningar (skyddat) ----------
    public async Task<List<Bokning>> GetBokningarAsync(CancellationToken ct = default)
    {
        AttachToken();
        return await _http.GetFromJsonAsync<List<Bokning>>("api/bokningar", JsonOpts, ct) ?? new();
    }

    public async Task<Bokning?> GetBokningAsync(int id, CancellationToken ct = default)
    {
        AttachToken();
        return await _http.GetFromJsonAsync<Bokning>($"api/bokningar/{id}", JsonOpts, ct);
    }

    public async Task<(bool ok, string? error)> CreateBokningAsync(BokningFormModel m, CancellationToken ct = default)
    {
        AttachToken();
        var payload = new
        {
            bordId = m.BordId,
            startTid = m.StartTid,
            antalGaster = m.AntalGaster,
            kundNamn = m.KundNamn,
            kundTelefon = m.KundTelefon
        };
        var resp = await _http.PostAsJsonAsync("api/bokningar", payload, ct);
        return (resp.IsSuccessStatusCode, resp.IsSuccessStatusCode ? null : await ReadErrorAsync(resp, ct));
    }

    public async Task<(bool ok, string? error)> UpdateBokningAsync(BokningFormModel m, CancellationToken ct = default)
    {
        AttachToken();
        var payload = new
        {
            bordId = m.BordId,
            startTid = m.StartTid,
            antalGaster = m.AntalGaster
        };
        var resp = await _http.PutAsJsonAsync($"api/bokningar/{m.Id}", payload, ct);
        return (resp.IsSuccessStatusCode, resp.IsSuccessStatusCode ? null : await ReadErrorAsync(resp, ct));
    }

    public async Task<bool> DeleteBokningAsync(int id, CancellationToken ct = default)
    {
        AttachToken();
        var resp = await _http.DeleteAsync($"api/bokningar/{id}", ct);
        return resp.IsSuccessStatusCode;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage resp, CancellationToken ct)
    {
        try
        {
            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            if (doc.ValueKind == JsonValueKind.Object && doc.TryGetProperty("message", out var msg))
                return msg.GetString() ?? "Ett fel uppstod.";
        }
        catch { /* ignoreras */ }

        return resp.StatusCode switch
        {
            HttpStatusCode.Conflict => "Bordet är upptaget vid vald tidpunkt.",
            HttpStatusCode.Unauthorized => "Du måste vara inloggad.",
            _ => "Ett fel uppstod vid anropet."
        };
    }
}
