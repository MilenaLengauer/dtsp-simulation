using System.Threading.Tasks;
using TSPSimulation.UserInterface.Navigation;

namespace TSPSimulation.UserInterface.Util
{
    public interface IBaseViewModel
    {
        public INavigationController NavigationController { get; set; }
        public string Heading { get; set; }

        public Task OnAfterNavigate();
    }
}
