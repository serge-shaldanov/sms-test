using System.Text;

using Microsoft.Extensions.Configuration;

using WireMock.Matchers;
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
        Port                = port,
        StartAdminInterface = true,
    }
);

string? testUser     = configuration[key: "Auth:Username"];
string? testPassword = configuration[key: "Auth:Password"];

if (testUser == null || testPassword == null)
{
    Console.WriteLine(value: "В appsettings.json не указано имя пользователя и пароль.");

    return;
}

string authHeaderValue =
    "Basic " + Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: $"{testUser}:{testPassword}"));

// Запрос 1 (успешный путь, с ценами).
server
   .Given(
        requestMatcher: Request.Create()
                               .WithPath("/api/menu")
                               .UsingPost()
                               .WithBody(
                                    matcher: new JsonMatcher(
                                        value: "{\"Command\": \"GetMenu\",\"CommandParameters\": {\"WithPrice\": true}}"
                                    )
                                )
                               .WithHeader(name: "Authorization", authHeaderValue)
    )
   .RespondWith(
        provider: Response.Create()
                          .WithStatusCode(code: 200)
                          .WithHeader(name: "Content-Type", "application/json")
                          .WithBody(
                               body:
                               "{\"Command\": \"GetMenu\",\"Success\": true,\"ErrorMessage\": \"\",\"Data\": {\"MenuItems\": [{\"Id\": \"5979224\",\"Article\": \"A1004292\",\"Name\": \"Каша гречневая\",\"Price\": 50,\"IsWeighted\": false,\"FullPath\": \"ПРОИЗВОДСТВО\\\\Гарниры\",\"Barcodes\": [\"57890975627974236429\"]},{\"Id\": \"9084246\",\"Article\": \"A1004293\",\"Name\": \"Конфеты Коровка\",\"Price\": 300,\"IsWeighted\": true,\"FullPath\": \"ДЕСЕРТЫ\\\\Развес\",\"Barcodes\": []}]}}"
                           )
    );

// Запрос 1 (успешный путь, без цен).
server
   .Given(
        requestMatcher: Request.Create()
                               .WithPath("/api/menu")
                               .UsingPost()
                               .WithBody(
                                    matcher: new JsonMatcher(
                                        value:
                                        "{\"Command\": \"GetMenu\",\"CommandParameters\": {\"WithPrice\": false}}"
                                    )
                                )
                               .WithHeader(name: "Authorization", authHeaderValue)
    )
   .RespondWith(
        provider: Response.Create()
                          .WithStatusCode(code: 200)
                          .WithHeader(name: "Content-Type", "application/json")
                          .WithBody(
                               body:
                               "{\"Command\": \"GetMenu\",\"Success\": true,\"ErrorMessage\": \"\",\"Data\": {\"MenuItems\": [{\"Id\": \"5979224\",\"Article\": \"A1004292\",\"Name\": \"Каша гречневая\",\"IsWeighted\": false,\"FullPath\": \"ПРОИЗВОДСТВО\\\\Гарниры\",\"Barcodes\": [\"57890975627974236429\"]},{\"Id\": \"9084246\",\"Article\": \"A1004293\",\"Name\": \"Конфеты Коровка\",\"IsWeighted\": true,\"FullPath\": \"ДЕСЕРТЫ\\\\Развес\",\"Barcodes\": []}]}}"
                           )
    );

// Запрос 1 (отсутствуют обязательные параметры).
server
   .Given(
        requestMatcher: Request.Create()
                               .WithPath("/api/menu")
                               .UsingPost()
                               .WithBody(
                                    matcher: new JsonMatcher(
                                        value: "{\"Command\": \"GetMenu\",\"CommandParameters\": {}}"
                                    )
                                )
                               .WithHeader(name: "Authorization", authHeaderValue)
    )
   .RespondWith(
        provider: Response.Create()
                          .WithStatusCode(code: 200)
                          .WithHeader(name: "Content-Type", "application/json")
                          .WithBody(
                               body:
                               "{\"Command\": \"GetMenu\",\"Success\": false,\"ErrorMessage\": \"Required params are missing: WithPrice.\"}"
                           )
    );

// Запрос 2 (успешный путь).
server
   .Given(
        requestMatcher: Request.Create()
                               .WithPath("/api/orders")
                               .UsingPut()
                               .WithBody(matcher: new JmesPathMatcher("length(CommandParameters.MenuItems) > `0`"))
                               .WithHeader(name: "Authorization", authHeaderValue)
    )
   .AtPriority(priority: 1)
   .RespondWith(
        provider: Response.Create()
                          .WithStatusCode(code: 200)
                          .WithHeader(name: "Content-Type", "application/json")
                          .WithBody(body: "{\"Command\": \"SendOrder\",\"Success\": true,\"ErrorMessage\": \"\"}")
    );

// Запрос 2 (отсутствует ИД заказа).
server
   .Given(
        requestMatcher: Request.Create()
                               .WithPath("/api/orders")
                               .UsingPut()
                               .WithBody(matcher: new JmesPathMatcher("length(CommandParameters.MenuItems) == `0`"))
                               .WithHeader(name: "Authorization", authHeaderValue)
    )
   .AtPriority(priority: 2)
   .RespondWith(
        provider: Response.Create()
                          .WithStatusCode(code: 200)
                          .WithHeader(name: "Content-Type", "application/json")
                          .WithBody(
                               body:
                               "{\"Command\": \"SendOrder\",\"Success\": false,\"ErrorMessage\": \"Required params are missing: MenuItems.\"}"
                           )
    );

Console.WriteLine(
    value: $"Сервер запущен на: {server.Url}.\n"                     +
           $"Диагностика запросов: {server.Url}/__admin/requests.\n" +
           $"Нажмите любую клавишу для остановки."
);

Console.ReadKey();

server.Stop();
