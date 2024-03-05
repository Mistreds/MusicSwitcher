using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicSwitcher.Model;
using MusicSwitcher.Services;
using MusicSwitcher.ViewModel;
using MusicSwitcher.WorkerServices;

namespace MusicSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IHost Host { get; private set; }

        public static T GetService<T>()
            where T : class
        {
            if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException(
                    $"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
            }

            return service;
        }

        public class ConsoleHelper
        {
           
            /// <summary>
            /// Allocates a new console for current process.
            /// </summary>
            [DllImport("kernel32.dll")]
            public static extern Boolean AllocConsole();

            /// <summary>
            /// Frees the console.
            /// </summary>
            [DllImport("kernel32.dll")]
            public static extern Boolean FreeConsole();
        }

        public App()
        {
#if DEBUG
             ConsoleHelper.AllocConsole();
#endif

            var _host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory);

            _host.ConfigureServices((context, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MusicModel>();
                services.AddSingleton<IMusicServices, MusicServices>();
                services.AddHostedService<MusicBackgroundServices>();

            });
            Host = _host.Build();
            
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            App.GetService<MainWindow>().Show();
            await Host.StartAsync();
            
            base.OnStartup(e);
        }
    }

}
