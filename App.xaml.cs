using ACCess.Services;
using ACCess.View;
using ACCess.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ACCess
{

    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App ()
        {
            Services = ConfigureServices();
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<IGameService, GameService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();

            // Views
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Services.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
