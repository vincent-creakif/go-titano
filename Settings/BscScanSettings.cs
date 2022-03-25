namespace Creakif.GoTitano.Settings;

public class BscScanSettings
{
    public string ApiBaseUrl { get; set; }
    public string ApiKey { get; set; }

    public Uri ApiBaseUri => new(ApiBaseUrl);
}
