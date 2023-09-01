using System.Threading.Tasks;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.Navigation
{

    public delegate void Navigation(IBaseViewModel viewModel);

    public class NavigationController : NotifyPropertyChanged, INavigationController
    {
        public event Navigation? OnNavigate;

        public async Task Navigate(IBaseViewModel viewModel)
        {
            OnNavigate?.Invoke(viewModel);
            await viewModel.OnAfterNavigate();
        }
    }
}
