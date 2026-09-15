using System.Text.Json.Serialization;

using SmsTest.OrdersApi.Contracts.Menu;

namespace SmsTest.OrdersApi.Rest.Client.Models.Menu;

internal sealed class GetMenuParams
{
    [JsonPropertyName(name: "WithPrice")]
    public bool WithPrice { get; set; }

    public static GetMenuParams FromContract(MenuOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new GetMenuParams
        {
            WithPrice = options.WithPrice,
        };
    }
}
