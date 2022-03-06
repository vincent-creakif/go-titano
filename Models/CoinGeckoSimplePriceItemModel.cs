using System.Reflection;

namespace Creakif.GoTitano.Models;

public class CoinGeckoSimplePriceItemModel
{
    public decimal Eur { get; set; }
    public decimal Usd { get; set; }

    [JsonPropertyName("last_updated_at")]
    public long LastUpdatedAtTimestamp { get; set; }

    [JsonIgnore]
    public string CoinId { get; set; }

    [JsonIgnore]
    public DateTime LastUpdatedAt => LastUpdatedAtTimestamp.UnixTimeStampToLocalDateTime();

    [JsonIgnore]
    public DateTimeOffset LastUpdatedAtLocalTime { get; set; }

    public decimal In(string currency)
    {
        var priceProperty = GetType().GetProperty(currency, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return (decimal)priceProperty.GetValue(this);
    }
    
    public bool HasChanged(decimal currentPrice, string currency) => currentPrice != In(currency);
}