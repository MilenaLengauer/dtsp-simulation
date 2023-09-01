using Autofac;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TSPSimulation.UserInterface.Navigation;
using TSPSimulation.UserInterface.ViewModels;

namespace TSPSimulation.UserInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private static IContainer BuildIoCContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<CreateProblemViewModel>().SingleInstance();
            builder.RegisterType<SimulationViewModel>();
            //builder.RegisterType<ShowProblemViewModel>();
            //builder.RegisterType<StartSimulationViewModel>().SingleInstance();
            builder.RegisterType<NavigationController>().SingleInstance().As<INavigationController>();

            return builder.Build();
        }

        public IContainer? Container { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var dateFormat = "yyyyMMddHHmmssffff";
            var logFileName = $"logs/application{DateTime.Now.ToString(dateFormat)}.log";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFileName)
                .CreateLogger();
            base.OnStartup(e);
            Container ??= BuildIoCContainer();

            Log.Logger.Information("Application started");
        }
    }
}
