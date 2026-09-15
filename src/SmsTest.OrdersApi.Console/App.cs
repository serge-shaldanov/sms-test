using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SmsTest.OrdersApi.Console.Data;
using SmsTest.OrdersApi.Contracts;
using SmsTest.OrdersApi.Contracts.Exceptions;
using SmsTest.OrdersApi.Contracts.Menu;
using SmsTest.OrdersApi.Contracts.Orders;

namespace SmsTest.OrdersApi.Console;

public sealed class App
{
    private readonly IHostApplicationLifetime _lifetime;

    private readonly ILogger<App> _logger;

    private readonly IOrdersApiClient _orders;

    private readonly IServiceScopeFactory _services;

    public App(
        IOrdersApiClient         ordersClient,
        IServiceScopeFactory     serviceProvider,
        IHostApplicationLifetime lifetime,
        ILogger<App>             logger
    )
    {
        ArgumentNullException.ThrowIfNull(ordersClient);
        this._orders = ordersClient;

        ArgumentNullException.ThrowIfNull(serviceProvider);
        this._services = serviceProvider;

        ArgumentNullException.ThrowIfNull(lifetime);
        this._lifetime = lifetime;

        ArgumentNullException.ThrowIfNull(logger);
        this._logger = logger;
    }

    public async Task RunAsync()
    {
        CancellationToken token = this._lifetime.ApplicationStopping;

        try
        {
            this._logger.LogInformation(message: "Приложение успешно запущено");
            await this.RunAsyncInternal(token);
            this._logger.LogInformation(message: "Работа приложения завершена");
        }
        catch (OperationCanceledException)
        {
            this._logger.LogInformation(message: "Работа приложения прервана...");
        }
        catch (Exception exception)
        {
            System.Console.WriteLine(value: "Произошла неожиданная ошибка. Приложение будет закрыто.");
            this._logger.LogError(exception, message: "Необработанная ошибка в приложении");
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                this._lifetime.StopApplication();
            }
        }
    }

    private async Task RunAsyncInternal(CancellationToken token)
    {
        // Развитие:
        // Сейчас решение простое, и вся логика лежит в одном файле.
        // При дальнейшем росте сложности и увеличении количества логики имеет смысл поступить следующим образом:
        // 1) Если это традиционная архитектура CLI (Команда -> Ответ), то переход на связку:
        // CmdLineParser + Command + Handler (можно использовать тот же Mediator, или написать собственный обработчик).
        // 2) Если это "богатое" консольное приложение (с интерфейсами, событийным лупом), то переход на связку:
        // ConsoleGui + StateMachine + State.
        // И обязательный вынос всей бизнес-логики во внешние библиотеки. Здесь оставляем только:
        // 1) работу с аргументами (если CLI)
        // 2) ввод / вывод на экран, формирование интерфейсов
        // 3) конфигурацию DI контейнера

        IList<MenuItem>? menuItems = await this.LoadMenuItemAsync(token);

        if (menuItems == null)
        {
            return;
        }

        await this.SyncMenuItemsAsync(token, menuItems);

        PrintMenuItems(menuItems);

        await this.MakeOrderAsync(token, menuItems);
    }

    private async Task<IList<MenuItem>?> LoadMenuItemAsync(CancellationToken token)
    {
        IList<MenuItem> menuItems;

        try
        {
            menuItems = await this._orders.GetMenuAsync(
                options: new MenuOptions
                {
                    WithPrice = true,
                },
                token
            );
        }
        catch (OperationFailedException exception)
        {
            // Перехватываем только бизнес-ошибки по контракту, как и описано в ТЗ.
            // В exception.Message — сообщение от API.
            // Остальные ошибки будут перехватываться глобальным обработчиком. 
            System.Console.WriteLine(value: $"Не удалось получить список блюд: {exception.Message}");

            return null;
        }

        return menuItems;
    }

    private async Task SyncMenuItemsAsync(CancellationToken token, IList<MenuItem> menuItems)
    {
        await using AsyncServiceScope scope = this._services.CreateAsyncScope();

        IOrdersRepository repository = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();

        await repository.UpdateMenuItemsAsync(menuItems, token);
    }

    private static void PrintMenuItems(IList<MenuItem> menuItems)
    {
        // В ТЗ указан вывод в формате:
        // Название – Код (артикул) – Цена за единицу
        // Развитие: красивый вывод через SpectreConsole (Table Widget).
        StringBuilder itemsBuilder = new StringBuilder();

        foreach (MenuItem item in menuItems)
        {
            if (itemsBuilder.Length > 0)
            {
                itemsBuilder.AppendLine();
            }

            itemsBuilder
               .Append(item.Name)
               .Append(value: " - ")
               .Append(item.Article)
               .Append(value: " - ")
               .Append(item.Price);
        }

        System.Console.WriteLine(value: itemsBuilder.ToString());
    }

    private async Task MakeOrderAsync(CancellationToken token, IList<MenuItem> menuItems)
    {
        Order order = new Order();

        IList<OrderItem> orderItems = this.QueryOrderItems(menuItems);

        if (orderItems.Count == 0)
        {
            return;
        }

        order.AddItems(orderItems);

        try
        {
            await this._orders.SendOrderAsync(order, token);
            System.Console.WriteLine(value: "УСПЕХ");
        }
        catch (OperationFailedException exception)
        {
            // Перехватываем только бизнес-ошибки по контракту, как и описано в ТЗ.
            // В exception.Message — сообщение от API.
            // Остальные ошибки будут перехватываться глобальным обработчиком. 
            System.Console.WriteLine(value: $"Не удалось создать заказ: {exception.Message}");
        }
    }

    private IList<OrderItem> QueryOrderItems(IList<MenuItem> menuItems)
    {
        Dictionary<string, MenuItem> allowedItems =
            menuItems.ToDictionary(keySelector: x => x.Article, elementSelector: x => x);

        while (true)
        {
            System.Console.Write(value: "Введите список блюд: ");
            string? response = System.Console.ReadLine();

            // Выходим, если введена пустая строка.
            if (string.IsNullOrEmpty(response))
            {
                return [];
            }

            // Можно через Span + IndexOf, но в данном случае это избыточно.
            string[]        rawItems = response.Split(separator: ';');
            List<OrderItem> result   = new List<OrderItem>(rawItems.Length);

            foreach (string item in rawItems)
            {
                string[] itemPair = item.Split(separator: ':', StringSplitOptions.TrimEntries);

                if (itemPair.Length != 2)
                {
                    System.Console.WriteLine(value: "Формат: Код1:Количество1;Код2:Количество2;Код3:Количество3;...");

                    break;
                }

                string article      = itemPair[0];
                string quantityText = itemPair[1];

                if (!allowedItems.ContainsKey(article))
                {
                    System.Console.WriteLine(value: "Требуется указать код существующего блюда.");

                    break;
                }

                if (!double.TryParse(quantityText, result: out double quantity) || quantity <= 0)
                {
                    System.Console.WriteLine(
                        value: $"Требуется указать положительное количество позиций для блюда \"{article}\"."
                    );

                    break;
                }

                result.Add(
                    item: new OrderItem
                    {
                        Id       = allowedItems[article].Id,
                        Quantity = quantity,
                    }
                );
            }

            if (result.Count < rawItems.Length)
            {
                // Обнаружены ошибки ввода.
                continue;
            }

            return result;
        }
    }
}
