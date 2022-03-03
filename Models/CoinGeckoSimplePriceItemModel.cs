namespace Creakif.GoTitano.Models;

public class CoinGeckoSimplePriceItemModel
{
    [JsonPropertyName("eur")]
    public decimal EurValue { get; set; }

    [JsonPropertyName("usd")]
    public decimal UsdValue { get; set; }

    [JsonPropertyName("last_updated_at")]
    public long LastUpdatedAtTimestamp { get; set; }

    [JsonIgnore]
    public string CoinId { get; set; }

    [JsonIgnore]
    public string EurValueFormatted { get; set; }

    [JsonIgnore]
    public string UsdValueFormatted { get; set; }

    [JsonIgnore]
    public DateTime LastUpdatedAt => LastUpdatedAtTimestamp.UnixTimeStampToLocalDateTime();

    [JsonIgnore]
    public DateTimeOffset LastUpdatedAtLocalTime { get; set; }  
    
    public bool HasChanged(decimal currentPriceInUsd) => UsdValue != currentPriceInUsd;
}