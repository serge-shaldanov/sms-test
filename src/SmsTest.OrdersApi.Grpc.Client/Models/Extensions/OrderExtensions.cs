using Sms.Test;

using OrderItem = Sms.Test.OrderItem;

namespace SmsTest.OrdersApi.Grpc.Client.Models.Extensions;

public static class OrderExtensions
{
    // В .NET 8 расширений для статических методов ещё нет, поэтому эмулируем их поведение.
    public static Order FromContract(Contracts.Orders.Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        IEnumerable<OrderItem> items = order.Items.Select(
            selector: oi => new OrderItem
            {
                Id       = oi.Id,
                Quantity = oi.Quantity,
            }
        );

        Order grpcOrder = new Order
        {
            Id = order.Id,
        };

        grpcOrder.OrderItems.AddRange(items);

        return grpcOrder;
    }
}
