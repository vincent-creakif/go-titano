namespace Creakif.GoTitano.Models;

public class BscScanResultModel
{
    [JsonPropertyName("result")]
    public IEnumerable<BscScanResultItemModel> Items { get; set; }
}

public class BscScanResultItemModel
{
    [JsonPropertyName("value")]
    public string ValueStr { get; set; }

    public string Timestamp { get; set; }

    [JsonIgnore]
    public decimal Value => ValueStr.FromWeiValue();

    [JsonIgnore]
    public DateTime CreatedAt => long.Parse(Timestamp).UnixTimeStampToLocalDateTime();
}