using System.Text.Json.Serialization;

namespace SmsTest.OrdersApi.Rest.Client.Models.Menu;

internal sealed class GetMenuResult
{
    [JsonPropertyName(name: "MenuItems")]
    public List<GetMenuItem> MenuItems { get; set; } = [];
}
