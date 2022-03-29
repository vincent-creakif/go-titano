using Blazored.LocalStorage;

namespace Creakif.GoTitano.Services;

public class LocalStorageService
{
    private const string CurrencyKey = "Currency";
    private const string WalletAddressKey = "WalletAddress";
    private const string WalletAddressHistoryKey = "WalletAddressHistory";

    private readonly ILocalStorageService _localStorageService;

    public LocalStorageService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<string> GetWalletAddressAsync(CancellationToken ct)
    {
        return await _localStorageService.GetItemAsync<string>(WalletAddressKey, ct);
    }

    public async Task StoreWalletAddressAsync(string walletAddress, CancellationToken ct)
    {
        await _localStorageService.SetItemAsync(WalletAddressKey, walletAddress, ct);
    }

    public async Task RemoveWalletAddressAsync(CancellationToken ct)
    {
        await _localStorageService.RemoveItemAsync(WalletAddressKey, ct);
    }

    public async Task<string> GetCurrencyAsync(CancellationToken ct)
    {
        return await _localStorageService.GetItemAsync<string>(CurrencyKey, ct);
    }

    public async Task StoreCurrencyAsync(string currency, CancellationToken ct)
    {
        await _localStorageService.SetItemAsync(CurrencyKey, currency, ct);
    }

    public async Task<IList<string>> GetWalletAddressHistoryAsync(CancellationToken ct)
    {
        return await _localStorageService.GetItemAsync<IList<string>>(WalletAddressHistoryKey, ct);
    }

    public async Task AddWalletAddressToHistoryIfNeededAsync(string walletAddress, CancellationToken ct)
    {
        var history = await GetWalletAddressHistoryAsync(ct) ?? new List<string>();
        if (!history.Contains(walletAddress))
        {
            history.Insert(0, walletAddress);
        }

        await _localStorageService.SetItemAsync(WalletAddressHistoryKey, history, ct);
    }
}
