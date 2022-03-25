namespace Creakif.GoTitano.Models;

public class State
{
    public CoinGeckoSimplePriceItemModel Price { get; set; }
    public TitanoBalancesModel Balances { get; set; }
    public TitanoEarningsModel Earnings { get; set; }
    public BscScanResultItemModel InitialBalance { get; set; }
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
        Balances = null;
        Earnings = null;
    }
}