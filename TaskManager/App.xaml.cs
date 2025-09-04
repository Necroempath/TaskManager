using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services;
using TaskManager.ViewModels;
using TaskManager.Views;

namespace TaskManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        ConfigureServices(services);
        
        _serviceProvider = services.BuildServiceProvider();
        
        var mainWindow = new TaskManagerWindow
        {
            DataContext = _serviceProvider.GetRequiredService<TaskManagerViewModel>()
        };
        
       mainWindow.Show();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<TaskManagerViewModel>();
        services.AddSingleton<ProcessesMonitoringService>();
        services.AddSingleton<StartProcessViewModel>();
    }
}