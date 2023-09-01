using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TSPSimulation.UserInterface.Navigation;
using TSPSimulation.UserInterface.ViewModels;

namespace TSPSimulation.UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var container = ((App)Application.Current).Container;

            var mainViewModel = container.Resolve<MainViewModel>();
            DataContext = mainViewModel;

            var navigationController = container.Resolve<INavigationController>();
            var createProblemViewModel = container.Resolve<CreateProblemViewModel>();

            // load initial view model
            this.Loaded += async (s, e) => await navigationController.Navigate(createProblemViewModel);
        }
    }
}
