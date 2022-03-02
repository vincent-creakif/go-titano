namespace Creakif.GoTitano.Services;

public class TitanoService
{
    private TimeZoneService _timeZoneService;

    private readonly CultureInfo _culture = CultureInfo.CreateSpecificCulture("en-US");

    private const decimal _dailyRoiInPercent = 1.89984m;
    private const int _rebaseFrequencyPerHour = 2;
    private const int _rebaseFrequencyPerDay = 24 * _rebaseFrequencyPerHour;
    private const decimal _rebaseRoi = (_dailyRoiInPercent / _rebaseFrequencyPerDay) / 100;

    private const int MaxForecastYears = 1;

    public static DateTime RebaseTime => DateTime.UtcNow.AddMinutes(5);

    public TitanoService(TimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService;
    }

    public TitanoBalancesModel GetBalances(string balance, decimal price)
    {
        if (decimal.TryParse(balance, NumberStyles.AllowDecimalPoint, _culture, out decimal balanceAmount))
        {
            return new(
                balanceAmount,
                balanceAmount * price);
        }
        return new(0, 0);
    }

    public TitanoEarningsModel GetEarnings(decimal price, decimal balance)
    {
        var rebaseAmount = balance * _rebaseRoi;
        var rebaseAmountValue = rebaseAmount * price;

        var dailyRebaseAmount = rebaseAmount * _rebaseFrequencyPerDay;
        var dailyRebaseAmountValue = rebaseAmountValue * _rebaseFrequencyPerDay;

        return new(
            rebaseAmount,
            rebaseAmountValue,
            dailyRebaseAmount,
            dailyRebaseAmountValue);
    }

    public async Task<IReadOnlyCollection<TitanoForecastItem>> GetForecastItems(decimal price, decimal balanceAmount)
    {
        var localDatetime = await _timeZoneService.GetLocalDateTime(RebaseTime);
        var startTime = localDatetime;

        var weekOffset = 0;

        var totalDays = (localDatetime.AddYears(MaxForecastYears) - localDatetime).TotalDays - 1;

        var remainingTime = TimeSpan.FromHours(24) - localDatetime.TimeOfDay;
        var remainingRebases = remainingTime.Hours * _rebaseFrequencyPerHour;
        remainingRebases += ((remainingTime.Minutes - remainingTime.Minutes % 30) / 30) + 1;

        TitanoForecastItem GetDailyForecast(int rebases)
        {
            var day = localDatetime;

            //var weekOffset = (int)Math.Floor((localDatetime.Date - startTime.Date).TotalDays / 7);
            if (day.DayOfWeek == DayOfWeek.Monday)
            {
                weekOffset++;
            }

            var dailyRebaseAmounts = new List<decimal>();
            var dailyRebaseAmountValues = new List<decimal>();
            var balanceAmountValue = 0m;
            for (var i = 0; i < rebases; i++)
            {
                var rebaseAmount = balanceAmount * _rebaseRoi;
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
            result.Add(GetDailyForecast(_rebaseFrequencyPerDay));
        }

        return result;
    }
}