using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SmsTest.VariablesApp.DI;
using SmsTest.VariablesApp.Models.Services;
using SmsTest.VariablesApp.Views;

using WpfBindingErrors;

namespace SmsTest.VariablesApp;

public partial class App
{
    private readonly IHost _host;

    private readonly ILogger<App> _logger;

    public App()
    {
        this._host = Host.CreateDefaultBuilder()
                         .ConfigureAppServices()
                         .Build();

        this._logger = this._host.Services.GetRequiredService<ILogger<App>>();

        BindingExceptionThrower.Attach();

        this.DispatcherUnhandledException          += this.OnUnhandledDispatcherException;
        TaskScheduler.UnobservedTaskException      += this.OnUnhandledTaskException;
        AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledDomainException;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            this._logger.LogInformation(message: "Приложение запущено");

            await this._host.StartAsync();

            MainWindow mainWindow = this._host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
        catch (Exception exc)
        {
            this._logger.LogError(exc, message: "Не удалось инициализировать приложение");
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            this._logger.LogInformation(message: "Приложение закрывается...");

            this.DispatcherUnhandledException          -= this.OnUnhandledDispatcherException;
            TaskScheduler.UnobservedTaskException      -= this.OnUnhandledTaskException;
            AppDomain.CurrentDomain.UnhandledException -= this.OnUnhandledDomainException;

            await this._host.StopAsync();
            this._host.Dispose();

            base.OnExit(e);
        }
        catch (Exception exc)
        {
            this._logger.LogError(exc, message: "Не удалось корректно закрыть приложение");
        }
    }

    private void OnUnhandledDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        this._logger.LogError(e.Exception, message: "Обнаружено необработанное исключение на диспетчере");

        this._host
            .Services
            .GetRequiredService<IDialogService>()
            .ShowErrorDialog(message: "Произошла неожиданная ошибка.");

        e.Handled = true;
    }

    private void OnUnhandledTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        this._logger.LogError(e.Exception, message: "Обнаружено необработанное исключение в одной из задач");

        e.SetObserved();
    }

    private void OnUnhandledDomainException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception? exception = e.ExceptionObject as Exception;

        this._logger.LogError(exception, message: "Обнаружено необработанное исключение на домене приложения");
    }
}
