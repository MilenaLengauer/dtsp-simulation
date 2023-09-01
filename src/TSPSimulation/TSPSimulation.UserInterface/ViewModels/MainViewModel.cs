using TSPSimulation.UserInterface.Navigation;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        public INavigationController NavigationController { get; set; }

        private IBaseViewModel? selectedViewModel;

        public IBaseViewModel? SelectedViewModel
        {
            get => this.selectedViewModel;
            set => Set(ref this.selectedViewModel, value);
        }

        public MainViewModel(INavigationController navigationController)
        {
            NavigationController = navigationController;
            NavigationController.OnNavigate += UpdateSelectedViewModel;
        }

        public void UpdateSelectedViewModel(IBaseViewModel viewModel)
        {
            SelectedViewModel = viewModel;
        }

    }
}
