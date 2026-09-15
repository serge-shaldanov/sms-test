using System.Text.Json.Serialization;

using SmsTest.OrdersApi.Contracts.Menu;

namespace SmsTest.OrdersApi.Rest.Client.Models.Menu;

internal sealed class GetMenuItem
{
    [JsonPropertyName(name: "Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName(name: "Article")]
    public string Article { get; set; } = string.Empty;

    [JsonPropertyName(name: "Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName(name: "Price")]
    public decimal? Price { get; set; }

    [JsonPropertyName(name: "IsWeighted")]
    public bool IsWeighted { get; set; }

    [JsonPropertyName(name: "FullPath")]
    public string FullPath { get; set; } = string.Empty;

    [JsonPropertyName(name: "Barcodes")]
    public List<string> Barcodes { get; set; } = [];

    public MenuItem ToContract()
    {
        return new MenuItem
        {
            Article    = this.Article,
            Barcodes   = [.. this.Barcodes],
            FullPath   = this.FullPath,
            Id         = this.Id,
            IsWeighted = this.IsWeighted,
            Name       = this.Name,
            Price      = this.Price ?? 0,
        };
    }
}
