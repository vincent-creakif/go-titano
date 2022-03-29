namespace Creakif.GoTitano.Models;

public record TitanoBalanceModel(
    decimal BalanceAmount,
    decimal BalanceAmountValue)
{
    public bool HasChanged(decimal currentBalanceAmount) => BalanceAmount != currentBalanceAmount;
};

public static class TitanoBalancesModelExtensions
{
    public static TitanoBalanceModel WithBalanceAmountValue(this TitanoBalanceModel titanoBalances, decimal balanceAmountValue)
    {
        return titanoBalances with { BalanceAmountValue = balanceAmountValue };
    }
}
