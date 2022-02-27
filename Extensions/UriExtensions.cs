namespace Creakif.GoTitano.Extensions;

public static class UriExtensions
{
    public static Uri CombineUri(this Uri uri, string relativeOrAbsoluteUri)
    {
        return new Uri(uri, relativeOrAbsoluteUri);
    }

    public static Uri CombineUri(this Uri uri, params string[] relativeOrAbsoluteUris)
    {
        var result = uri;
        foreach (var relativeOrAbsoluteUri in relativeOrAbsoluteUris)
        {
            result = result.CombineUri(relativeOrAbsoluteUri);
        }

        return result;
    }

    public static Uri Append(this Uri uri, params string[] paths)
    {
        return new Uri(paths.Aggregate(uri.AbsoluteUri,
            (current, path) => $"{current.TrimEnd('/')}/{HttpUtility.UrlEncode(path)}"));
    }

    public static Uri AppendHash(this Uri uri, string hashPath)
    {
        return new Uri($"{uri.AbsoluteUri.TrimEnd('#')}#{hashPath.TrimStart('#')}");
    }

    public static Uri AppendParameter(this Uri uri, string name, string value)
    {
        var values = new Dictionary<string, string> { { name, value } };
        return uri.AppendParameters(values);
    }

    public static Uri AppendParameters(this Uri uri, IDictionary<string, string> values)
    {
        var baseUrl = uri.ToString();
        var queryString = string.Empty;
        if (baseUrl.Contains("?"))
        {
            var urlSplit = baseUrl.Split('?');
            baseUrl = urlSplit[0];
            queryString = urlSplit.Length > 1 ? urlSplit[1] : string.Empty;
        }

        NameValueCollection queryCollection = HttpUtility.ParseQueryString(queryString);
        foreach (var kvp in values ?? new Dictionary<string, string>())
        {
            queryCollection[kvp.Key] = kvp.Value;
        }
        var uriKind = uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative;
        return queryCollection.Count == 0
            ? new Uri(baseUrl, uriKind)
            : new Uri($"{baseUrl}?{queryCollection}", uriKind);
    }
}
