using System.Text.Json.Serialization;

namespace SmsTest.OrdersApi.Rest.Client.Models.Orders;

internal sealed class SendOrderMenuItem
{
    [JsonPropertyName(name: "Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName(name: "Quantity")]
    public double Quantity { get; set; }
}
