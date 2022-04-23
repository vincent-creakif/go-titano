namespace Creakif.GoTitano.Constants;

public static class Coins
{
    public const string Titano = "titano";

    public static readonly Dictionary<string, CoinMetadata> Metadata = new()
    {
        { Titano, new("0X4E3CABD3AD77420FF9031D19899594041C420AEE", "binance-smart-chain") }
    };

    public record CoinMetadata(
        string Contract,
        string CoinGeckoPlatformId);
}
