using System.Windows;
using Core.Interfaces;
using Desktop.ViewModels;
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

    protected override void OnStartup(StartupEventArgs e)
    {
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
        services.AddSingleton<IConfigurationRoot>(config);

        services.AddSingleton<IServiceRegistration, Infrastructure.ServiceRegistration>();

        // Вызов регистрации инфраструктурных сервисов
        var serviceRegistration = services.BuildServiceProvider().GetRequiredService<IServiceRegistration>();
        serviceRegistration.ConfigureServices(services, config);

        // Регистрация ViewModels
        services.AddTransient<ProductViewModel>();
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
}