namespace Creakif.GoTitano.Models;

public record TitanoBalancesModel(
    decimal BalanceAmount,
    decimal BalanceAmountValue)
{
    public bool HasChanged(decimal currentBalanceAmount) => BalanceAmount != currentBalanceAmount;
};