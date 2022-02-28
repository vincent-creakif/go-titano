namespace Creakif.GoTitano.Extensions;

public static class DecimalExtensions
{
    private static readonly CultureInfo _culture = CultureInfo.CreateSpecificCulture("en-US");

    public static decimal FromWeiValue(this string weiValue)
    {
        var left = weiValue[..(weiValue.Length - 18)];
        var right = weiValue[^18..];

        return decimal.Parse($"{left}.{right}", _culture);
    }
}