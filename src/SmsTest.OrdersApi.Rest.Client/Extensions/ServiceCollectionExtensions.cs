using System.Net.Http.Headers;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

using SmsTest.OrdersApi.Contracts;

namespace SmsTest.OrdersApi.Rest.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRestOrdersApiServices(
        this IServiceCollection services,
        RestOrdersApiOptions    options
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        string authString = Convert.ToBase64String(
            inArray: Encoding.UTF8.GetBytes(s: $"{options.UserName}:{options.Password}")
        );

        services.AddHttpClient<IOrdersApiClient, RestOrdersApiClient>(
            configureClient: client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(scheme: "Basic", authString);
            }
        );

        return services;
    }
}
