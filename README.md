# Smakbordet – MVC-webbplats

Publik webbplats och administratörsgränssnitt för restaurangen Smakbordet.
Byggt med ASP.NET Core 8 MVC och konsumerar [Restaurang Bokning API](https://github.com/Jakob12291/RestaurangBokning).

## Funktioner

- Startsida med presentation och populära rätter (hämtas från API:et).
- Menysida som visar hela menyn.
- Bokningssida där besökare kan söka lediga bord för datum, tid och antal gäster.
- Administratörsgränssnitt (under `/Admin`) för att hantera bord, meny och bokningar.
- Inloggning för administratörer via API:ets JWT, lagrad i en cookie.
- Responsiv design med Bootstrap och egen CSS, samt grundläggande SEO
  (meta-taggar, JSON-LD och `robots.txt`).

## Kom igång

Krav: [.NET 8 SDK](https://dotnet.microsoft.com/download) och att API:et körs.

```bash
# Återställ paket och bygg
dotnet build

# Kör webbplatsen
dotnet run
```

Webbplatsen startar på den URL som visas (t.ex. `http://localhost:5290`).
API:ets adress konfigureras i `appsettings.json` under `Api:BaseUrl`
(standard `http://localhost:5144`).

## Inloggning

Administratör (samma som i API:et):

| Användarnamn | Lösenord   |
|--------------|------------|
| `admin`      | `Admin123!`|

## Projektstruktur

```
Controllers/        Home, Meny, Bokning, Account
Areas/Admin/        Administratörsgränssnitt (Bord, Meny, Bokningar)
Models/             API-modeller och vy-modeller
Services/           RestaurantApiClient (typad HttpClient mot API:et)
Views/              Razor-vyer
wwwroot/            CSS, bilder, robots.txt
```
