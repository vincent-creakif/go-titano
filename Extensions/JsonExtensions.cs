namespace Creakif.GoTitano.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _caseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static TResult JsonDeserializeCaseInsensitive<TResult>(this string jsonData)
    {
        return JsonSerializer.Deserialize<TResult>(jsonData, _caseInsensitiveOptions);
    }
}