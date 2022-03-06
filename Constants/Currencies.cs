namespace Creakif.GoTitano.Constants;

public static class Currencies
{
    public const string Eur = "eur";
    public const string Usd = "usd";
    
    public static readonly Dictionary<string, string> Symbols = new()
    {
        { Eur, "€" },
        { Usd, "$" }
    };
}

public static class CurrenciesGroups
{
    public static string[] CurrenciesAvailable = new[]
    {
        Currencies.Eur,
        Currencies.Usd
    };
}
