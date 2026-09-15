using SmsTest.OrdersApi.Contracts.Menu;
using SmsTest.OrdersApi.Contracts.Orders;

namespace SmsTest.OrdersApi.Contracts;

public interface IOrdersApiClient
{
    Task<IList<MenuItem>> GetMenuAsync(MenuOptions options, CancellationToken token);

    Task SendOrderAsync(Order order, CancellationToken token);
}
