using Google.Protobuf.WellKnownTypes;

using Sms.Test;

using SmsTest.OrdersApi.Contracts;
using SmsTest.OrdersApi.Contracts.Exceptions;
using SmsTest.OrdersApi.Contracts.Menu;
using SmsTest.OrdersApi.Grpc.Client.Models.Extensions;

using MenuItem = SmsTest.OrdersApi.Contracts.Menu.MenuItem;
using Order = SmsTest.OrdersApi.Contracts.Orders.Order;

namespace SmsTest.OrdersApi.Grpc.Client;

internal sealed class GrpcOrdersApiClient : IOrdersApiClient
{
    private readonly SmsTestService.SmsTestServiceClient _client;

    public GrpcOrdersApiClient(SmsTestService.SmsTestServiceClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        this._client = client;
    }

    public async Task<IList<MenuItem>> GetMenuAsync(MenuOptions options, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(options);

        GetMenuResponse menu = await this._client
                                         .GetMenuAsync(
                                              request: new BoolValue
                                              {
                                                  Value = options.WithPrice,
                                              },
                                              cancellationToken: token
                                          )
                                         .ConfigureAwait(continueOnCapturedContext: false);

        return menu.Success
            ? [.. menu.MenuItems.Select(selector: mi => mi.ToContract())]
            : throw new OperationFailedException(message: menu.ErrorMessage ?? "Failed to obtain menu items.");
    }

    public async Task SendOrderAsync(Order order, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(order);

        Sms.Test.Order request = OrderExtensions.FromContract(order);

        SendOrderResponse response = await this._client
                                               .SendOrderAsync(request, cancellationToken: token)
                                               .ConfigureAwait(continueOnCapturedContext: false);

        if (!response.Success)
        {
            throw new OperationFailedException(
                message: response.ErrorMessage ?? $"Failed to create order \"{order.Id}\"."
            );
        }
    }
}
