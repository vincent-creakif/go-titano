namespace Creakif.GoTitano.Services;

public class TitanoService
{
    private const decimal DailyRoiInPercent = 1.9176m / 100;
    private const int RebaseFrequencyPerHour = 2;
    private const int RebaseFrequencyPerDay = 24 * RebaseFrequencyPerHour;
    private const decimal RebaseRoi = DailyRoiInPercent / RebaseFrequencyPerDay;

    private const int MaxForecastDurationInYears = 2;

    private TimeZoneService _timeZoneService;

    public static DateTime RebaseTime => DateTime.UtcNow.AddMinutes(5).AddSeconds(1);

    public TitanoService(TimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService;
    }

    public TitanoBalanceModel GetBalance(decimal balance, decimal price)
    {
        return new(
            balance,
            balance * price);
    }

    public TitanoEarningsModel GetEarnings(decimal price, decimal balance)
    {
        var rebaseAmount = balance * RebaseRoi;
        var rebaseAmountValue = rebaseAmount * price;

        var dailyRebaseAmount = rebaseAmount * RebaseFrequencyPerDay;
        var dailyRebaseAmountValue = rebaseAmountValue * RebaseFrequencyPerDay;

        return new(
            rebaseAmount,
            rebaseAmountValue,
            dailyRebaseAmount,
            dailyRebaseAmountValue);
    }

    public async Task<IReadOnlyCollection<TitanoForecastItem>> GetForecastItemsAsync(decimal price, decimal balanceAmount)
    {
        var localDatetime = await _timeZoneService.GetLocalDateTime(RebaseTime);

        var weekOffset = 0;

        var targetDate = localDatetime.AddYears(MaxForecastDurationInYears);
        // Set target date to the last day of the current month MaxForecastDurationInYears ahead
        targetDate = new DateTime(targetDate.Year, targetDate.Month, DateTime.DaysInMonth(targetDate.Year, targetDate.Month)).Date;

        var totalDays = (targetDate - localDatetime).TotalDays - 1;

        var remainingTime = TimeSpan.FromHours(24) - localDatetime.TimeOfDay;
        var remainingRebases = remainingTime.Hours * RebaseFrequencyPerHour;
        remainingRebases += ((remainingTime.Minutes - remainingTime.Minutes % 30) / 30) + 1;

        var numberOfDays = 0;
        TitanoForecastItem GetDailyForecast(int rebases)
        {
            var day = localDatetime;
            if (numberOfDays++ == 7)
            {
                numberOfDays = 1;
                weekOffset++;
            }

            var dailyRebaseAmounts = new List<decimal>();
            var dailyRebaseAmountValues = new List<decimal>();
            var balanceAmountValue = 0m;
            for (var i = 0; i < rebases; i++)
            {
                var rebaseAmount = balanceAmount * RebaseRoi;
                var rebaseAmountValue = rebaseAmount * price;

                balanceAmount += rebaseAmount;
                balanceAmountValue = balanceAmount * price;

                dailyRebaseAmounts.Add(rebaseAmount);
                dailyRebaseAmountValues.Add(rebaseAmountValue);

                localDatetime = localDatetime.AddMinutes(30);
            }

            return new(
                weekOffset,
                day,
                dailyRebaseAmounts,
                dailyRebaseAmountValues,
                balanceAmount,
                balanceAmountValue);
        }

        // Processing remaining rebases for the current day
        var result = new List<TitanoForecastItem>
        {
            GetDailyForecast(remainingRebases)
        };

        // Processing all future days on a year basis
        for (var i = 0; i < totalDays; i++)
        {
            result.Add(GetDailyForecast(RebaseFrequencyPerDay));
        }

        return result;
    }
}