using System.Text.Json.Serialization;

namespace SmsTest.OrdersApi.Rest.Client.Models.Common;

internal class ApiResponse
{
    [JsonPropertyName(name: "Command")]
    public string Command { get; set; } = string.Empty;

    [JsonPropertyName(name: "Success")]
    public bool Success { get; set; }

    [JsonPropertyName(name: "ErrorMessage")]
    public string? ErrorMessage { get; set; }
}
