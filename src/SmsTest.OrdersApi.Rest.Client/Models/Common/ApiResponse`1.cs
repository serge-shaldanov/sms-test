using System.Text.Json.Serialization;

namespace SmsTest.OrdersApi.Rest.Client.Models.Common;

internal sealed class ApiResponse<TResult> : ApiResponse
{
    [JsonPropertyName(name: "Data")]
    public TResult? Data { get; set; }
}
