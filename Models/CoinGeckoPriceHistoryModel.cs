namespace Creakif.GoTitano.Models;

public record CoinGeckoPriceHistoryModel(
    string CoinId,
    string Currency,
    IReadOnlyCollection<CoinGeckoPriceHistoryItemModel> History);

public record CoinGeckoPriceHistoryItemModel(
    DateTime CreatedAt,
    decimal Price);

public record CoinGeckoPriceHistoryItemWithCurrencyModel(
    string Currency,
    DateTime CreatedAt,
    decimal Price);

public static class CoinGeckoPriceHistoryModelExtensions
{
    public static IReadOnlyCollection<CoinGeckoPriceHistoryItemWithCurrencyModel> ToHistoryWithCurrencyModel(
        this IReadOnlyCollection<CoinGeckoPriceHistoryItemModel> history,
        string currency)
    {
        return history
            .Select(x => new CoinGeckoPriceHistoryItemWithCurrencyModel(currency, x.CreatedAt, x.Price))
            .ToList();
    }
}