namespace Creakif.GoTitano.Models;

public class BscScanTokenBalanceResultModel
{
    [JsonPropertyName("result")]
    public string ResultStr { get; set; }

    [JsonIgnore]
    public decimal Result => ResultStr.FromWeiValue();
}
