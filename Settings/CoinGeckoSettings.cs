namespace Creakif.GoTitano.Settings;

public class CoinGeckoSettings
{
    public string ApiBaseUrl { get; set; }
    public string HistoryPath { get; set; }

    public Uri ApiBaseUri => new(ApiBaseUrl);
}
