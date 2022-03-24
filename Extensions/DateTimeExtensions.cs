namespace Creakif.GoTitano.Extensions;

public static class DateTimeExtensions
{
    public static DateTime UnixTimeStampToDateTime(this long unixTimestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            .AddSeconds(unixTimestamp);
    }

    public static DateTime UnixTimeStampToUtcDateTime(this long unixTimestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(unixTimestamp);
    }

    public static DateTime UnixTimeStampToLocalDateTime(this long unixTimestamp)
    {
        return UnixTimeStampToUtcDateTime(unixTimestamp)
            .ToLocalTime();
    }
}