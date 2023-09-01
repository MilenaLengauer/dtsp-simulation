using System.Linq;
using System.Windows.Navigation;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class ResultTourViewModel : NotifyPropertyChanged
    {
        public ShowProblemViewModel ShowProblemViewModel { get; private set; }
        public DynamicTSPSolution Solution { get; private set; }

        private ReachedCity? selectedCity;
        public ReachedCity? SelectedCity { 
            get => selectedCity; 
            set
            {
                Set(ref selectedCity, value);
                if(selectedCity != null) ShowProblemViewModel.SliderValue = selectedCity.Time;
            }
        }

        public ResultTourViewModel(DynamicTSPSolution solution)
        {
            Solution = solution;
            ShowProblemViewModel = new ShowProblemViewModel(solution.DynamicTSP, solution, false);
        }

    }
}
