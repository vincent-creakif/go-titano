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
    public static IReadOnlyCollection<DateTime> Months(this IReadOnlyCollection<TitanoForecastItem> ForecastItems, int offset)
    {
        var firstYear = ForecastItems.First().Day.Year;
        var currentYear = firstYear + offset;

        return ForecastItems
            .Where(x => x.Day.Year == currentYear)
            .Select(x => new DateTime(x.Day.Year, x.Day.Month, 1))
            .Distinct()
            .ToList();
    }

    public static IReadOnlyCollection<DateTime> WeekDays(this IReadOnlyCollection<TitanoForecastItem> ForecastItems, int offset)
    {
        var firstWeekNumber = ForecastItems.First().WeekNumber;
        var currentYear = ForecastItems.First().Day.Year;
        var currentWeekNumber = firstWeekNumber + offset;
       
        if (currentWeekNumber > 52)
        {
            // 53 -> 
            // currentWeekNumber - 52 = 1
            // 110
            // currentWeekNumber - 52 = 58

            var test = (currentWeekNumber - 52) % 52;
            currentYear += ((currentWeekNumber - currentWeekNumber % 52) / 52);
            currentWeekNumber = 1;
        }

        return ForecastItems
            .Where(x => x.Day.Year == currentYear && x.WeekNumber == firstWeekNumber + offset)
            .Select(x => x.Day.Date)
            .ToList();
    }
}
