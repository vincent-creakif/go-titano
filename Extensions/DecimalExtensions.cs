namespace Creakif.GoTitano.Extensions;

public static class DecimalExtensions
{
    private static readonly CultureInfo _culture = CultureInfo.CreateSpecificCulture("en-US");

    public static decimal FromWeiValue(this string weiValue)
    {
        return decimal.Parse(weiValue[..^18] + "." + weiValue[^18..], _culture);
    }
}