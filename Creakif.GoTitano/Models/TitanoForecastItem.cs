namespace Creakif.GoTitano.Models;

public record TitanoForecastItem(
    int WeekNumber,
    DateTimeOffset Day,
    IEnumerable<decimal> DailyRebaseAmounts,
    IEnumerable<decimal> DailyRebaseAmountValues,
    decimal BalanceAmount,
    decimal BalanceAmountValue);

public static class TitanoForecastItemsExtensions
{
    public static IReadOnlyCollection<DateTime> Months(this IReadOnlyCollection<TitanoForecastItem> ForecastItems)
    {
        return ForecastItems
            .Select(x => new DateTime(x.Day.Year, x.Day.Month, 1))
            .Distinct()
            .ToList();
    }
}
