namespace SmsTest.OrdersApi.Contracts.Menu;

public sealed class MenuItem
{
    public required string Id { get; set; }

    public required string Article { get; set; }

    public required string Name { get; set; }

    public required decimal Price { get; set; }

    public required bool IsWeighted { get; set; }

    public required string FullPath { get; set; }

    public required List<string> Barcodes { get; set; } = [];
}
