namespace Creakif.GoTitano.Models;

public record TitanoForecastItem(
    int WeekOffset,
    DateTimeOffset Day,
    IEnumerable<decimal> DailyRebaseAmounts,
    IEnumerable<decimal> DailyRebaseAmountValues,
    decimal BalanceAmount,
    decimal BalanceAmountValue);

public static class TitanoForecastItemsExtensions
{
    public static IReadOnlyCollection<DateTime> Months(this IReadOnlyCollection<TitanoForecastItem> forecastItems, int offset)
    {
        var firstYear = forecastItems.First().Day.Year;
        var currentYear = firstYear + offset;

        return forecastItems
            .Where(x => x.Day.Year == currentYear)
            .Select(x => new DateTime(x.Day.Year, x.Day.Month, 1))
            .Distinct()
            .ToList();
    }

    public static int YearlyPageCount(this IReadOnlyCollection<TitanoForecastItem> forecastItems)
    {
        return forecastItems.Select(x => x.Day.Year).Distinct().Count();
    }

    public static int WeeklyPageCount(this IReadOnlyCollection<TitanoForecastItem> forecastItems)
    {
        return (int)Math.Ceiling((forecastItems.Last().Day - forecastItems.First().Day).TotalDays / 7);
    }

    public static IReadOnlyCollection<DateTime> WeekDays(this IReadOnlyCollection<TitanoForecastItem> forecastItems, int offset)
    {
        return forecastItems
            .Where(x => x.WeekOffset == offset)
            .Select(x => x.Day.Date)
            .ToList();
    }
}
