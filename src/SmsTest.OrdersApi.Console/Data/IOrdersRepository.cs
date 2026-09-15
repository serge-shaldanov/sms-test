using SmsTest.OrdersApi.Contracts.Menu;

namespace SmsTest.OrdersApi.Console.Data;

public interface IOrdersRepository
{
    Task UpdateMenuItemsAsync(IEnumerable<MenuItem> items, CancellationToken token);
}
