namespace SmsTest.OrdersApi.Rest.Client.Extensions;

public sealed class RestOrdersApiOptions
{
    public required string BaseUrl { get; set; }

    public required string UserName { get; set; }

    public required string Password { get; set; }
}
