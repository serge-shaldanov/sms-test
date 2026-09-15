using Microsoft.Extensions.DependencyInjection;

using Sms.Test;

using SmsTest.OrdersApi.Contracts;

namespace SmsTest.OrdersApi.Grpc.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcOrdersApiServices(
        this IServiceCollection services,
        GrpcOrdersApiOptions    options
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services.AddGrpcClient<SmsTestService.SmsTestServiceClient>(
                     configureClient: o =>
                     {
                         o.Address = new Uri(options.BaseUrl);
                     }
                 )
                .ConfigurePrimaryHttpMessageHandler(
                     configureHandler: () => new SocketsHttpHandler
                     {
                         UseProxy                       = false,
                         EnableMultipleHttp2Connections = true,
                     }
                 );

        services.AddScoped<IOrdersApiClient, GrpcOrdersApiClient>();

        return services;
    }
}
