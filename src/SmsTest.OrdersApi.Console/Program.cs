using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using SmsTest.OrdersApi.Console;
using SmsTest.OrdersApi.Console.Data;
using SmsTest.OrdersApi.Console.Tracing;
#if USE_GRPC_CLIENT
using SmsTest.OrdersApi.Grpc.Client.Extensions;

#else
using SmsTest.OrdersApi.Rest.Client.Extensions;
#endif

// Развитие: вынесение групп сервисов в экстеншен-методы IServiceCollection.
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration
       .SetBasePath(basePath: Directory.GetCurrentDirectory())
       .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables();

builder.Services.AddLogging(
    configure: b =>
    {
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                                                 .ReadFrom
                                                 .Configuration(builder.Configuration);

        b.ClearProviders()
         .AddSerilog(logger: loggerConfiguration.CreateLogger());
    }
);

// Смена протокола управляется флагом компиляции:
// Если задан USE_GRPC_CLIENT — использовать gRPC-клиент,
// иначе использовать REST-клиент.
#if USE_GRPC_CLIENT
builder.Services.AddGrpcOrdersApiServices(
    options: builder.Configuration.GetSection(key: "Api:Grpc").Get<GrpcOrdersApiOptions>()!
);
#else
builder.Services.AddRestOrdersApiServices(
    options: builder.Configuration.GetSection(key: "Api:Rest").Get<RestOrdersApiOptions>()!
);
#endif

builder.Services.AddDbContext<OrdersDbContext>(
    optionsAction: options =>
        options.UseNpgsql(connectionString: builder.Configuration.GetConnectionString(name: "DefaultConnection"))
);

builder.Services.AddScoped<IOrdersRepository, DbContextOrdersRepository>();

builder.Services.AddScoped<App>();

IHost host = builder.Build();

Console.SetOut(
    newOut: new LoggableConsoleWriter(Console.Out, logger: host.Services.GetRequiredService<ILogger<App>>())
);

Console.SetIn(
    newIn: new LoggableTextReader(Console.In, logger: host.Services.GetRequiredService<ILogger<App>>())
);

using IServiceScope scope = host.Services.CreateScope();
OrdersDbContext     db    = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
await db.Database.MigrateAsync();

App app = scope.ServiceProvider.GetRequiredService<App>();

// Можно просто app.RunAsync(), но хочется поддерживать нативные прерывание от хоста (по Ctrl+C).
// Все ошибки в RunAsync перехватываются собственным обработчиком, так что Unobserved Exception тут не страшен.
_ = Task.Run(app.RunAsync);

await host.RunAsync();
