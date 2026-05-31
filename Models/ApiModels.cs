namespace RestaurangMVC.Models;

public class MenyItem
{
    public int Id { get; set; }
    public string Namn { get; set; } = string.Empty;
    public decimal Pris { get; set; }
    public string? Beskrivning { get; set; }
    public bool IsPopular { get; set; }
    public string? BildUrl { get; set; }
}

public class Bord
{
    public int Id { get; set; }
    public int Bordsnummer { get; set; }
    public int Kapacitet { get; set; }
}

public class Bokning
{
    public int Id { get; set; }
    public int BordId { get; set; }
    public int Bordsnummer { get; set; }
    public int KundId { get; set; }
    public string KundNamn { get; set; } = string.Empty;
    public DateTime StartTid { get; set; }
    public DateTime SlutTid { get; set; }
    public int AntalGaster { get; set; }
}

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
