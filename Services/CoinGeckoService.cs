using Microsoft.Extensions.Caching.Memory;

namespace Creakif.GoTitano.Services;

public class CoinGeckoService
{
    private const string MonthlyHistoryCacheKey = "CoinGeckoMonthlyHistory";

    private static readonly HttpClient _httpClient = new();

    private static readonly TimeSpan _cacheDelay = TimeSpan.FromMinutes(30);
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

    private readonly CoinGeckoSettings _settings;
    private readonly TimeZoneService _timeZoneService;

    public CoinGeckoService(IOptions<CoinGeckoSettings> settings, TimeZoneService timeZoneService)
    {
        _settings = settings.Value;
        _timeZoneService = timeZoneService;
    }

    public async Task<IReadOnlyCollection<CoinGeckoPriceHistoryModel>> GetMonthlyPriceHistoryAsync(string coinId, CancellationToken ct)
    {
        var cacheKey = $"{MonthlyHistoryCacheKey}${coinId}";

        // If found in cache, return result directly
        if (_cache.TryGetValue(cacheKey, out IReadOnlyCollection<CoinGeckoPriceHistoryModel> cachedHistory))
        {
            return cachedHistory;
        }

        var history = new List<CoinGeckoPriceHistoryModel>();
        foreach (var currency in CurrenciesGroups.CurrenciesAvailable)
        {
            var historyPath = Path.Combine(_settings.HistoryPath, coinId, currency);
            var files = Directory.GetFiles(historyPath, "*.json");

            foreach (var file in files)
            {
                await using FileStream fileStream = File.OpenRead(file);

                history.Add(new(
                    coinId,
                    currency,
                    await JsonSerializer.DeserializeAsync<IReadOnlyCollection<CoinGeckoPriceHistoryItemModel>>(fileStream, cancellationToken: ct)));
            }
        }

        // Set history in memory cache
        _cache.Set(cacheKey, history, _cacheDelay);

        return history;
    }

    public async Task<IReadOnlyCollection<CoinGeckoPriceHistoryItemWithCurrencyModel>> GetPriceHistoryValueAsync(
        string coinId,
        DateTime dateTime,
        CancellationToken ct)
    {
        dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour - 1, 45, 0);

        return (await GetMonthlyPriceHistoryAsync(coinId, ct))
            .Where(x => x.CoinId == coinId)
            .SelectMany(x => x.History.ToHistoryWithCurrencyModel(x.Currency))
            .Where(x => x.CreatedAt > dateTime && x.CreatedAt < dateTime.AddHours(1))
            .ToList();
    }

    public async Task GetMonthlyMarketChartAsync(string coinId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        // For the first day of the month, we make sure to get all the history of the previous month
        if (now.Day == 1)
        {
            now = now.AddMonths(-1);
        }

        foreach (var currency in CurrenciesGroups.CurrenciesAvailable)
        {
            var month = new DateTimeOffset(new DateTime(now.Year, now.Month, 1));
            var totalMonths = ((now.Year * 12 + now.Month) - (month.Year * 12 + month.Month)) + 1;
            for (var i = 0; i < totalMonths; i++)
            {
                var lastDayOfMonth = DateTime.DaysInMonth(month.Year, month.Month);
                var from = new DateTimeOffset(new DateTime(month.Year, month.Month, 1, 0, 0, 0)).ToUnixTimeSeconds();
                var to = new DateTimeOffset(new DateTime(month.Year, month.Month, lastDayOfMonth, 23, 59, 59)).ToUnixTimeSeconds();

                var uri = _settings.ApiBaseUri
                    .Append("coins", Coins.Metadata[coinId].PlatformId, "contract", Coins.Metadata[coinId].Contract, "market_chart", "range")
                    .AppendParameter("vs_currency", currency)
                    .AppendParameter("from", from.ToString())
                    .AppendParameter("to", to.ToString());

                try
                {
                    var response = await _httpClient.GetAsync(uri, ct);
                    var responseContent = await response.Content.ReadAsStringAsync(ct);

                    var result = responseContent.JsonDeserializeCaseInsensitive<CoinGeckoMarketChartItemModel>();

                    var historyResult = new List<CoinGeckoPriceHistoryItemModel>();
                    foreach (var item in result.Items)
                    {
                        var firstValue = item.ElementAt(0).ToString()[0..^3];
                        var secondValue = item.ElementAt(1).ToString();

                        historyResult.Add(new(
                            long.Parse(firstValue).UnixTimeStampToDateTime(),
                            decimal.Parse(secondValue)));
                    }

                    var historyPath = Path.Combine(_settings.HistoryPath, coinId, currency);
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

                month = month.AddMonths(1);
            }
        }
    }

    public async Task<CoinGeckoSimplePriceItemModel> GetTitanoPriceAsync(CancellationToken ct)
    {
        var result = await GetSimplePriceAsync(new[] { Coins.Titano }, ct);

        return result?.SingleOrDefault(x => x.CoinId.Equals(Coins.Titano));
    }

    private async Task<IReadOnlyCollection<CoinGeckoSimplePriceItemModel>> GetSimplePriceAsync(string[] coinIds, CancellationToken ct)
    {
        var uri = _settings.ApiBaseUri
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