using System.ComponentModel.DataAnnotations;

namespace RestaurangMVC.Models;

public class HomeViewModel
{
    public List<MenyItem> PopuläraRätter { get; set; } = new();
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Ange användarnamn")]
    [Display(Name = "Användarnamn")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ange lösenord")]
    [DataType(DataType.Password)]
    [Display(Name = "Lösenord")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class BordFormModel
{
    public int Id { get; set; }

    [Required, Display(Name = "Bordsnummer")]
    [Range(1, 1000, ErrorMessage = "Ange ett giltigt bordsnummer")]
    public int Bordsnummer { get; set; }

    [Required, Display(Name = "Kapacitet (antal platser)")]
    [Range(1, 100, ErrorMessage = "Kapaciteten måste vara mellan 1 och 100")]
    public int Kapacitet { get; set; }
}

public class MenyFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ange namn"), Display(Name = "Namn")]
    [StringLength(120)]
    public string Namn { get; set; } = string.Empty;

    [Required, Display(Name = "Pris (kr)")]
    [Range(0, 100000, ErrorMessage = "Ange ett giltigt pris")]
    public decimal Pris { get; set; }

    [Display(Name = "Beskrivning")]
    [StringLength(500)]
    public string? Beskrivning { get; set; }

    [Display(Name = "Populär rätt")]
    public bool IsPopular { get; set; }

    [Display(Name = "Bild-URL")]
    [StringLength(500)]
    [Url(ErrorMessage = "Ange en giltig webbadress")]
    public string? BildUrl { get; set; }
}

public class BokningFormModel
{
    public int Id { get; set; }

    [Required, Display(Name = "Bord")]
    public int BordId { get; set; }

    [Required, Display(Name = "Datum och tid")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime StartTid { get; set; } = DateTime.Today.AddHours(18);

    [Required, Display(Name = "Antal gäster")]
    [Range(1, 100)]
    public int AntalGaster { get; set; } = 2;

    [Required(ErrorMessage = "Ange kundens namn"), Display(Name = "Kundens namn")]
    public string KundNamn { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ange telefonnummer"), Display(Name = "Telefonnummer")]
    public string KundTelefon { get; set; } = string.Empty;

    public List<Bord> TillgängligaBord { get; set; } = new();
}

public class LedigaBordViewModel
{
    [Required, Display(Name = "Datum")]
    [DataType(DataType.Date)]
    public DateTime Datum { get; set; } = DateTime.Today;

    [Required, Display(Name = "Tid")]
    [DataType(DataType.Time)]
    public TimeSpan Tid { get; set; } = new(18, 0, 0);

    [Required, Display(Name = "Antal gäster")]
    [Range(1, 100)]
    public int AntalGaster { get; set; } = 2;

    public List<Bord>? Resultat { get; set; }
    public bool HarSökt { get; set; }
}
