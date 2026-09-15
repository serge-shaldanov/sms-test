using System.Text.Json.Serialization;

namespace SmsTest.OrdersApi.Rest.Client.Models.Common;

internal sealed class ApiRequest<TCommandParams>
{
    [JsonPropertyName(name: "Command")]
    public string Command { get; set; } = string.Empty;

    [JsonPropertyName(name: "CommandParameters")]
    public TCommandParams? CommandParameters { get; set; }
}
