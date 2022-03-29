namespace Creakif.GoTitano.Models;

public class BscScanInitialBalanceResultModel
{
    [JsonPropertyName("result")]
    public IEnumerable<BscScanInitialBalanceResultItemModel> Items { get; set; }
}

public class BscScanInitialBalanceResultItemModel
{
    [JsonPropertyName("value")]
    public string ValueStr { get; set; }

    public string Timestamp { get; set; }

    [JsonIgnore]
    public decimal Value => ValueStr.FromWeiValue();

    [JsonIgnore]
    public DateTime CreatedAt => long.Parse(Timestamp).UnixTimeStampToLocalDateTime();
}