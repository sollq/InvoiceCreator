using System.Windows;
using System.Windows.Threading;
using Core.Interfaces;
using Desktop.ViewModels;
using Desktop.Views;
using Infrastructure.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Desktop;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    private ILogger? _logger;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += App_DispatcherUnhandledException;

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", true)
            .Build();

        var services = new ServiceCollection();

        // Логирование
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        services.AddSingleton<IConfiguration>(config);
        services.AddSingleton(config);

        services.AddSingleton<IServiceRegistration, ServiceRegistration>();

        // Вызов регистрации инфраструктурных сервисов
        var serviceRegistration = services.BuildServiceProvider().GetRequiredService<IServiceRegistration>();
        serviceRegistration.ConfigureServices(services, config);

        // Регистрация ViewModels
        services.AddSingleton<ProductViewModel>();
        services.AddTransient<InvoiceInputViewModels>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        // Создание и настройка главного окна
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.DataContext = mainViewModel;

        // Инициализация ViewModel
        _ = Task.Run(async () =>
        {
            try
            {
                await mainViewModel.InitAsync();
            }
            catch (Exception ex)
            {
                var logger = _serviceProvider.GetRequiredService<ILogger<App>>();
                logger.LogError(ex, "Ошибка при инициализации приложения");
            }
        });

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Предотвращаем падение приложения
        e.Handled = true;

        var exception = e.Exception;
        _logger?.LogError(exception, "Произошла необработанная ошибка");
        
        // Показываем пользователю сообщение об ошибке
        MessageBox.Show($"Произошла критическая ошибка. Приложение продолжит работу, но некоторые функции могут быть недоступны.\n\nОшибка: {exception.Message}", 
                        "Критическая ошибка", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
    }
    
    public void SetLogger(ILogger logger)
    {
        _logger = logger;
    }
}