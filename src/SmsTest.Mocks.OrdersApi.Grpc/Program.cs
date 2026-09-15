using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.Configuration;

using Sms.Test;

using WireMock.Net.Google.Protobuf.Request;
using WireMock.Net.Google.Protobuf.Response;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

IConfigurationRoot configuration = new ConfigurationBuilder()
                                  .SetBasePath(basePath: Directory.GetCurrentDirectory())
                                  .AddJsonFile(path: "appsettings.json", optional: false)
                                  .Build();

string? portText = configuration[key: "Server:Port"];

if (!int.TryParse(portText, result: out int port))
{
    Console.WriteLine(value: "В appsettings.json не указан корректный порт сервера.");

    return;
}

WireMockServer server = WireMockServer.Start(
    settings: new WireMockServerSettings
    {
        Port     = port,
        UseSSL   = false,
        UseHttp2 = true,
    }
);

// Запрос 1 (успешный путь, с ценами).
server
   .Given(
        requestMatcher: Request.Create()
                               .UsingPost()
                               .WithPath("/sms.test.SmsTestService/GetMenu")
                               .WithBodyAsGoogleProtobuf(
                                    message: new BoolValue
                                    {
                                        Value = true,
                                    }
                                )
    )
   .RespondWith(
        provider: Response.Create()
                          .WithHeader(name: "Content-Type", "application/grpc")
                          .WithTrailingHeader(name: "grpc-status", "0")
                          .WithBodyAsGoogleProtobuf(
                               responseMessage: new GetMenuResponse
                               {
                                   Success = true,
                                   MenuItems =
                                   {
                                       new MenuItem
                                       {
                                           Id         = "5979224",
                                           Article    = "A1004292",
                                           Name       = "Каша гречневая",
                                           Price      = 50,
                                           IsWeighted = false,
                                           FullPath   = "ПРОИЗВОДСТВО\\Гарниры",
                                           Barcodes =
                                           {
                                               "57890975627974236429",
                                           },
                                       },
                                       new MenuItem
                                       {
                                           Id         = "9084246",
                                           Article    = "A1004293",
                                           Name       = "Конфеты Коровка",
                                           Price      = 300,
                                           IsWeighted = true,
                                           FullPath   = "ДЕСЕРТЫ\\Развес",
                                       },
                                   },
                               }
                           )
    );

// Запрос 1 (успешный путь, без цен).
server
   .Given(
        requestMatcher: Request.Create()
                               .UsingPost()
                               .WithPath("/sms.test.SmsTestService/GetMenu")
                               .WithBodyAsGoogleProtobuf(
                                    message: new BoolValue
                                    {
                                        Value = false,
                                    }
                                )
    )
   .RespondWith(
        provider: Response.Create()
                          .WithHeader(name: "Content-Type", "application/grpc")
                          .WithTrailingHeader(name: "grpc-status", "0")
                          .WithBodyAsGoogleProtobuf(
                               responseMessage: new GetMenuResponse
                               {
                                   Success = true,
                                   MenuItems =
                                   {
                                       new MenuItem
                                       {
                                           Id         = "5979224",
                                           Article    = "A1004292",
                                           Name       = "Каша гречневая",
                                           IsWeighted = false,
                                           FullPath   = "ПРОИЗВОДСТВО\\Гарниры",
                                           Barcodes =
                                           {
                                               "57890975627974236429",
                                           },
                                       },
                                       new MenuItem
                                       {
                                           Id         = "9084246",
                                           Article    = "A1004293",
                                           Name       = "Конфеты Коровка",
                                           IsWeighted = true,
                                           FullPath   = "ДЕСЕРТЫ\\Развес",
                                       },
                                   },
                               }
                           )
    );

// Запрос 2 (успешный путь).
server
   .Given(
        requestMatcher: Request.Create()
                               .UsingPost()
                               .WithPath("/sms.test.SmsTestService/SendOrder")
                               .WithBodyAsGoogleProtobuf<Order>(predicate: order => order.OrderItems.Count > 0)
    )
   .RespondWith(
        provider: Response.Create()
                          .WithHeader(name: "Content-Type", "application/grpc")
                          .WithTrailingHeader(name: "grpc-status", "0")
                          .WithBodyAsGoogleProtobuf(
                               responseMessage: new SendOrderResponse
                               {
                                   Success = true,
                               }
                           )
    );

// Запрос 2 (отсутствует часть параметров).
server
   .Given(
        requestMatcher: Request.Create()
                               .UsingPost()
                               .WithPath("/sms.test.SmsTestService/SendOrder")
    )
   .AtPriority(priority: 10)
   .RespondWith(
        provider: Response.Create()
                          .WithHeader(name: "Content-Type", "application/grpc")
                          .WithTrailingHeader(name: "grpc-status", "0")
                          .WithBodyAsGoogleProtobuf(
                               responseMessage: new SendOrderResponse
                               {
                                   Success      = false,
                                   ErrorMessage = "Required parameters are missing: order_items.",
                               }
                           )
    );

Console.WriteLine(
    value: $"Сервер запущен на: {server.Url}.\n" +
           $"Нажмите любую клавишу для остановки."
);

Console.ReadKey();

server.Stop();
