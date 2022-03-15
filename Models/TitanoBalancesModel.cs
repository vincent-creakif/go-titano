namespace Creakif.GoTitano.Models;

public record TitanoBalancesModel(
    decimal BalanceAmount,
    decimal BalanceAmountValue)
{
    public bool HasChanged(decimal currentBalanceAmount) => BalanceAmount != currentBalanceAmount;
};

public static class TitanoBalancesModelExtensions
{
    public static TitanoBalancesModel WithBalanceAmountValue(this TitanoBalancesModel titanoBalances, decimal balanceAmountValue)
    {
        return titanoBalances with { BalanceAmountValue = balanceAmountValue };
    }
}
