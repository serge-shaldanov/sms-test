using System.Text.Json.Serialization;

using SmsTest.OrdersApi.Contracts.Orders;

namespace SmsTest.OrdersApi.Rest.Client.Models.Orders;

internal sealed class SendOrderParams
{
    [JsonPropertyName(name: "OrderId")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName(name: "MenuItems")]
    public List<SendOrderMenuItem> MenuItems { get; set; } = [];

    public static SendOrderParams FromContract(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return new SendOrderParams
        {
            OrderId = order.Id,
            MenuItems =
            [
                .. order.Items.Select(
                    selector: oi => new SendOrderMenuItem
                    {
                        Id       = oi.Id,
                        Quantity = oi.Quantity,
                    }
                ),
            ],
        };
    }
}
