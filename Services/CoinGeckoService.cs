namespace Creakif.GoTitano.Services;

public class CoinGeckoService
{
    private static readonly HttpClient _httpClient = new();

    private readonly Uri _apiBaseUri = new("https://api.coingecko.com/api/v3");

    private readonly TimeZoneService _timeZoneService;

    private readonly string _historyPath;

    public CoinGeckoService(IConfiguration configuration, TimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService;
        _historyPath = configuration.GetValue<string>("CoinGeckoHistoryPath");
    }

    public async Task GetMonthlyMarketChartAsync(
        string coinId,
        string currency,
        DateTimeOffset month,
        CancellationToken ct)
    {
        var lastDayOfMonth = DateTime.DaysInMonth(month.Year, month.Month);
        var from = new DateTimeOffset(new DateTime(month.Year, month.Month, 1, 0, 0, 0)).ToUnixTimeSeconds();
        var to = new DateTimeOffset(new DateTime(month.Year, month.Month, lastDayOfMonth, 23, 59, 59)).ToUnixTimeSeconds();

        var uri = _apiBaseUri
            .Append("coins", Coins.Metadata[coinId].PlatformId, "contract", Coins.Metadata[coinId].Contract, "market_chart", "range")
            .AppendParameter("vs_currency", currency)
            .AppendParameter("from", from.ToString())
            .AppendParameter("to", to.ToString());

        try
        {
            var response = await _httpClient.GetAsync(uri, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            var result = responseContent.JsonDeserializeCaseInsensitive<CoinGeckoMarketChartItemModel>();

            var historyResult = new List<CoinGeckoHistoryItemModel>();
            foreach (var item in result.Items)
            {
                var firstValue = item.ElementAt(0).ToString()[0..^3];
                var secondValue = item.ElementAt(1).ToString();

                historyResult.Add(new(
                    long.Parse(firstValue).UnixTimeStampToDateTime(),
                    decimal.Parse(secondValue)));
            }

            var historyPath = Path.Combine(_historyPath, coinId, currency);
            if (!Directory.Exists(historyPath))
            {
                Directory.CreateDirectory(historyPath);
            }

            var filePath = Path.Combine(historyPath, $"{month:yyyy-MM}.json");
            await using FileStream createStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(createStream, historyResult, cancellationToken: ct);
        }
        catch (Exception)
        {

        }
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
            .AppendParameter("vs_currencies", string.Join(",", CurrenciesGroups.CurrenciesAvailable))
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