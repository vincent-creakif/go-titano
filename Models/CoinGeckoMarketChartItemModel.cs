namespace Creakif.GoTitano.Models;

public class CoinGeckoMarketChartItemModel
{
    [JsonPropertyName("prices")]
    public IEnumerable<IEnumerable<object>> Items { get; set; }
}
