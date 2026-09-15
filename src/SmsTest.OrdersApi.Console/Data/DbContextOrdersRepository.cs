using PgBulk.EFCore;

using SmsTest.OrdersApi.Contracts.Menu;

namespace SmsTest.OrdersApi.Console.Data;

internal sealed class DbContextOrdersRepository : IOrdersRepository
{
    private readonly OrdersDbContext _context;

    public DbContextOrdersRepository(OrdersDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this._context = context;
    }

    public Task UpdateMenuItemsAsync(IEnumerable<MenuItem> items, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(items);

        // В ТЗ явно не указано, что делать, когда в БД уже существуют блюда с таким же ID.
        // Было сделано предположение, что мы должны обновить все существующие блюда по ключу, если они уже существуют.
        // Здесь используется эффективный bulk-merge через PgBulk (missing -> INSERT, exists -> UPDATE).
        // Можно собрать вручную, скажем, через StringBuilder + ExecuteSqlRawAsync:
        // INSERT INTO ... VALUES (...) ON CONFLICT (""Id"") DO UPDATE SET ...
        // Но это очень трудоёмко + нужно аккуратно экранировать параметры + в ТЗ явно не указано ограничение
        // на использование только чистого SQL.
        // Ещё вариант - экспорт во временную таблицу и COPY BINARY.
        // Обычные Find / Add / Remove от EF нам тут не подходят, поскольку на большом количестве блюд у нас будет куча
        // лишних запросов к БД.
        return this._context.BulkMergeAsync(items, cancellationToken: token);
    }
}
