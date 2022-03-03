namespace Creakif.GoTitano.Services;

public class CoinGeckoService
{
    private static readonly HttpClient _httpClient = new();

    private readonly Uri _apiBaseUri = new("https://api.coingecko.com/api/v3");
    private readonly CultureInfo _culture = CultureInfo.CreateSpecificCulture("en-US");
    
    private TimeZoneService _timeZoneService;

    public CoinGeckoService(TimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService;
    }    
    
    public async Task<CoinGeckoSimplePriceItemModel> GetTitanoPriceAsync(CancellationToken ct)
    {
        var result = await GetSimplePriceAsync(new[] { Coins.Titano }, ct);

        return result?.SingleOrDefault(x => x.CoinId.Equals(Coins.Titano));
    }

    private async Task<IReadOnlyCollection<CoinGeckoSimplePriceItemModel>> GetSimplePriceAsync(string[] coinIds, CancellationToken ct)
    {
        var uri = _apiBaseUri
            .Append("simple", "price")
            .AppendParameter("ids", string.Join(",", coinIds))
            .AppendParameter("vs_currencies", "eur,usd")
            .AppendParameter("include_last_updated_at", "true");

        try
        {
            var response = await _httpClient.GetAsync(uri, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            var priceItems = responseContent.JsonDeserializeCaseInsensitive<Dictionary<string, CoinGeckoSimplePriceItemModel>>();

            var results = new List<CoinGeckoSimplePriceItemModel>();
            foreach (var coinId in priceItems.Keys)
            {
                var resultItem = priceItems[coinId];
                resultItem.CoinId = coinId;
                resultItem.EurValueFormatted = resultItem.EurValue.ToString("N4", _culture);
                resultItem.UsdValueFormatted = resultItem.UsdValue.ToString("N4", _culture);
                resultItem.LastUpdatedAtLocalTime = await _timeZoneService.GetLocalDateTime(resultItem.LastUpdatedAt);

                results.Add(resultItem);
            }

            return results;
        }
        catch (Exception)
        {

        }

        return null;
    }
}