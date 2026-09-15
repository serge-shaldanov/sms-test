using SmsTest.OrdersApi.Contracts.Menu;

namespace SmsTest.OrdersApi.Grpc.Client.Models.Extensions;

public static class MenuItemExtensions
{
    public static MenuItem ToContract(this Sms.Test.MenuItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new MenuItem
        {
            Article    = item.Article,
            Barcodes   = [.. item.Barcodes],
            FullPath   = item.FullPath,
            Id         = item.Id,
            IsWeighted = item.IsWeighted,
            Name       = item.Name,
            Price      = (decimal) item.Price,
        };
    }
}
