namespace Creakif.GoTitano.Services;

public class BscScanService
{
    private const string TotalHoldersRegex = @"\$?(\d{1,3}(\,\d{3})*|(\d+))(\.\d{1,2})? addresses";

    private static readonly HttpClient _httpClient = new();

    private readonly Uri _baseUri = new("https://goto.bscscan.com/");

    private BscScanSettings _settings;
    private TimeZoneService _timeZoneService;

    public BscScanService(IOptions<BscScanSettings> settings, TimeZoneService timeZoneService)
    {
        _settings = settings.Value;
        _timeZoneService = timeZoneService;
    }       
    
    public async Task<BscScanResultItemModel> GetInitialBalanceAsync(
        string walletAddress,
        string contractAddress,
        CancellationToken ct)
    {
        var uri = _settings.ApiBaseUri
            .AppendParameter("apikey", _settings.ApiKey)
            .AppendParameter("module", "account")
            .AppendParameter("action", "tokentx")
            .AppendParameter("address", walletAddress)
            .AppendParameter("contractaddress", contractAddress)
            .AppendParameter("startblock", "0")
            .AppendParameter("endblock", "99999999")
            .AppendParameter("offset", "1")
            .AppendParameter("sort", "asc");

        try
        {
            var response = await _httpClient.GetAsync(uri, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            return (responseContent.JsonDeserializeCaseInsensitive<BscScanResultModel>()).Items.First();
        }
        catch (Exception)
        {

        }

        return null;
    }

    public async Task<BscScanHoldersModel> GetTotalHoldersAsync(string contractAddress, CancellationToken ct)
    {
        var uri = _baseUri.Append("token", contractAddress);

        try
        {
            var response = await _httpClient.GetAsync(uri, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            var totalHoldersMatches = Regex.Match(responseContent, TotalHoldersRegex);
            if (totalHoldersMatches.Success)
            {
                var totalHolders = totalHoldersMatches.Groups[1].Value.Replace(",", "");
                if (decimal.TryParse(totalHolders, out var totalHoldersValue))
                {
                    return new(
                        totalHoldersValue,
                        await _timeZoneService.GetLocalDateTime(DateTime.UtcNow));
                }
            }

            return default;
        }
        catch (Exception)
        {

        }

        return null;
    }    
}