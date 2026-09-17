using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using SmsTest.VariablesApp.Models.Services;
using SmsTest.VariablesApp.ViewModels;
using SmsTest.VariablesApp.Views;

namespace SmsTest.VariablesApp.DI;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureAppServices(this IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureAppConfiguration(ConfigureSettings);
        builder.ConfigureServices(ConfigureServices);

        return builder;
    }

    private static void ConfigureSettings(HostBuilderContext context, IConfigurationBuilder configuration)
    {
        configuration
           .SetBasePath(basePath: Directory.GetCurrentDirectory())
           .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true);
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        ConfigureLoggerServices(context, services);
        ConfigureVariableServices(context, services);
        ConfigureCommonUiServices(services);
        ConfigureMainWindowServices(services);
    }

    private static void ConfigureLoggerServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddLogging(
            configure: b =>
            {
                LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                                                         .ReadFrom
                                                         .Configuration(context.Configuration);

                b.ClearProviders()
                 .AddSerilog(logger: loggerConfiguration.CreateLogger());
            }
        );
    }

    private static void ConfigureVariableServices(HostBuilderContext context, IServiceCollection services)
    {
        services.Configure<VariableOptions>(config: context.Configuration.GetSection(VariableOptions.SectionName));

        services.AddSingleton<IVariableCommentStorageService, PersistentVariableCommentStorageService>();
        services.AddSingleton<IVariablesService, EnvironmentVariablesService>();
    }

    private static void ConfigureCommonUiServices(IServiceCollection services)
    {
        services.AddTransient<IDialogService, WindowsDialogService>();
    }

    private static void ConfigureMainWindowServices(IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }
}
