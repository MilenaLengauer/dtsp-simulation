using System.Threading.Tasks;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.Navigation
{
    public interface INavigationController
    {
        public event Navigation? OnNavigate;

        public Task Navigate(IBaseViewModel viewModel);

    }
}
