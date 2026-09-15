namespace SmsTest.OrdersApi.Contracts.Orders;

public sealed class Order
{
    private readonly Dictionary<string, OrderItem> _items;

    public Order(string orderId)
    {
        ArgumentException.ThrowIfNullOrEmpty(orderId);
        this.Id     = orderId;
        this._items = [];
    }

    public Order()
    {
        this.Id     = Guid.NewGuid().ToString(format: "D");
        this._items = [];
    }

    public string Id { get; }

    public IReadOnlyList<OrderItem> Items => [.. this._items.Values];

    public void AddItems(IEnumerable<OrderItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        foreach (OrderItem item in items)
        {
            if (this._items.TryGetValue(item.Id, value: out OrderItem? existingItem))
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                this._items.Add(item.Id, item);
            }
        }
    }
}
