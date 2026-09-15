using Microsoft.EntityFrameworkCore;

using SmsTest.OrdersApi.Contracts.Menu;

namespace SmsTest.OrdersApi.Console.Data;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    public DbSet<MenuItem> MenuItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<MenuItem>(
            buildAction: e =>
            {
                e.HasKey(keyExpression: d => d.Id);

                e.Property(propertyExpression: p => p.Price).HasPrecision(precision: 18, scale: 2);

                e.Property(propertyExpression: d => d.Barcodes).HasColumnType(typeName: "text[]");
            }
        );
    }
}
