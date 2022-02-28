namespace Creakif.GoTitano.Services;

public class BscScanService
{
    private const string ApiKey = "R48Q73AZQM64G63PB7SDFUGAT8X38BDJ9E";

    private static readonly HttpClient _httpClient = new();

    private readonly Uri _apiBaseUri = new("https://api.bscscan.com/api");

    public async Task<BscScanResultItemModel> GetInitialBalanceAsync(string walletAddress, CancellationToken ct)
    {
        var uri = _apiBaseUri
            .AppendParameter("apikey", ApiKey)
            .AppendParameter("module", "account")
            .AppendParameter("action", "tokentx")
            .AppendParameter("address", walletAddress)
            .AppendParameter("contractaddress", Coins.Contracts[Coins.Titano])
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
}