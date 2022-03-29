namespace Creakif.GoTitano.Models;

public class State
{
    public CoinGeckoSimplePriceItemModel Price { get; set; }
    public TitanoBalanceModel Balance { get; set; }
    public TitanoEarningsModel Earnings { get; set; }
    public BscScanInitialBalanceResultItemModel InitialBalance { get; set; }
    public IReadOnlyCollection<CoinGeckoPriceHistoryItemWithCurrencyModel> InitialPrices { get; set; }
    public string Currency { get; set; }
    public string CurrencySymbol => Currencies.Symbols[Currency];

    public State(string currency)
    {
        Currency = currency;
    }

    public void Clear()
    {
        Price = null;
        Balance = null;
        Earnings = null;
    }
}