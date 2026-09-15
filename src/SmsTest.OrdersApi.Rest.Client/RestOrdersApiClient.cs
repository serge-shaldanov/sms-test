using System.Net.Http.Json;

using SmsTest.OrdersApi.Contracts;
using SmsTest.OrdersApi.Contracts.Exceptions;
using SmsTest.OrdersApi.Contracts.Menu;
using SmsTest.OrdersApi.Contracts.Orders;
using SmsTest.OrdersApi.Rest.Client.Models.Common;
using SmsTest.OrdersApi.Rest.Client.Models.Menu;
using SmsTest.OrdersApi.Rest.Client.Models.Orders;

namespace SmsTest.OrdersApi.Rest.Client;

internal sealed class RestOrdersApiClient : IOrdersApiClient
{
    private readonly HttpClient _http;

    public RestOrdersApiClient(HttpClient http)
    {
        ArgumentNullException.ThrowIfNull(http);
        this._http = http;
    }

    public async Task SendOrderAsync(Order order, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(order);

        ApiRequest<SendOrderParams> request = new ApiRequest<SendOrderParams>
        {
            Command           = "SendOrder",
            CommandParameters = SendOrderParams.FromContract(order),
        };

        HttpResponseMessage response = await this._http
                                                 .PutAsJsonAsync(requestUri: "api/orders", request, token)
                                                 .ConfigureAwait(continueOnCapturedContext: false);

        // На всякий случай.
        if (!response.IsSuccessStatusCode)
        {
            throw new OperationFailedException(message: $"Failed to send order \"{order.Id}\".");
        }

        ApiResponse? result = await response
                                   .Content
                                   .ReadFromJsonAsync<ApiResponse>(token)
                                   .ConfigureAwait(continueOnCapturedContext: false);

        if (result is not
            {
                Success: true,
            })
        {
            throw new OperationFailedException(
                message: result?.ErrorMessage ?? $"Failed to create order \"{order.Id}\"."
            );
        }
    }

    public async Task<IList<MenuItem>> GetMenuAsync(MenuOptions options, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(options);

        ApiRequest<GetMenuParams> request = new ApiRequest<GetMenuParams>
        {
            Command           = "GetMenu",
            CommandParameters = GetMenuParams.FromContract(options),
        };

        HttpResponseMessage response = await this._http
                                                 .PostAsJsonAsync(requestUri: "api/menu", request, token)
                                                 .ConfigureAwait(continueOnCapturedContext: false);

        if (!response.IsSuccessStatusCode)
        {
            throw new OperationFailedException(message: "Failed to obtain menu items.");
        }

        ApiResponse<GetMenuResult>? result = await response
                                                  .Content
                                                  .ReadFromJsonAsync<ApiResponse<GetMenuResult>>(token)
                                                  .ConfigureAwait(continueOnCapturedContext: false);

        if (result is not
            {
                Success: true,
            } ||
            result.Data == null)
        {
            throw new OperationFailedException(message: result?.ErrorMessage ?? "Failed to obtain menu items.");
        }

        return [.. result.Data.MenuItems.Select(selector: i => i.ToContract())];
    }
}
