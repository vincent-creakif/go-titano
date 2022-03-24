namespace Creakif.GoTitano.Constants;

public static class Coins
{
    public const string Titano = "titano";

    public static readonly Dictionary<string, CoinMetadata> Metadata = new()
    {
        { Titano, new("0xBA96731324dE188ebC1eD87ca74544dDEbC07D7f", "binance-smart-chain") }
    };

    public record CoinMetadata(
        string Contract,
        string PlatformId);
}
