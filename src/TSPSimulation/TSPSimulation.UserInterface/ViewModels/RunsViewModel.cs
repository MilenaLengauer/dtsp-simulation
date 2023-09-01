using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Util;
using TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels;
using TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.ALPS;
using TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.NGA;

namespace TSPSimulation.UserInterface.ViewModels
{
    public class RunsViewModel : NotifyPropertyChanged
    {
        public RunModel RunModel { get; private set; }

        //public ICommand ExportRunCommand { get; }

        private DynamicTSPSolution? selectedSolution;
        public DynamicTSPSolution? SelectedSolution
        {
            get => this.selectedSolution;
            set {
                this.selectedSolution = value;
                UpdateSelectedRun();
            }
        }

        private AlgorithmParametersViewModel? algorithmParametersViewModel;
        public AlgorithmParametersViewModel? AlgorithmParametersViewModel
        {
            get => this.algorithmParametersViewModel;
            private set => Set(ref this.algorithmParametersViewModel, value);
        }

        private ResultTourViewModel? resultTourViewModel;
        public ResultTourViewModel? ResultTourViewModel
        {
            get => this.resultTourViewModel;
            private set => Set(ref this.resultTourViewModel, value);
        }

        private FitnessPerGenerationViewModel? fitnessPerGenerationViewModel;
        public FitnessPerGenerationViewModel? FitnessPerGenerationViewModel
        {
            get => this.fitnessPerGenerationViewModel;
            private set => Set(ref this.fitnessPerGenerationViewModel, value);
        }

        private DiversityPerGenerationViewModel? diversityPerGenerationViewModel;
        public DiversityPerGenerationViewModel? DiversityPerGenerationViewModel
        {
            get => this.diversityPerGenerationViewModel;
            private set => Set(ref this.diversityPerGenerationViewModel, value);
        }

        private DiversityHeatMapViewModel? diversityHeatMapViewModel;
        public DiversityHeatMapViewModel? DiversityHeatMapViewModel
        {
            get => this.diversityHeatMapViewModel;
            private set => Set(ref this.diversityHeatMapViewModel, value);
        }

        private EdgeFrequenciesViewModel? edgeFrequenciesViewModel;
        public EdgeFrequenciesViewModel? EdgeFrequenciesViewModel
        {
            get => this.edgeFrequenciesViewModel;
            private set => Set(ref this.edgeFrequenciesViewModel, value);
        }

        private EdgeFrequenciesBestKnownViewModel? edgeFrequenciesBestKnownViewModel;
        public EdgeFrequenciesBestKnownViewModel? EdgeFrequenciesBestKnownViewModel
        {
            get => this.edgeFrequenciesBestKnownViewModel;
            private set => Set(ref this.edgeFrequenciesBestKnownViewModel, value);
        }

        private PeriodsViewModel? periodsViewModel;
        public PeriodsViewModel? PeriodsViewModel
        {
            get => this.periodsViewModel;
            private set => Set(ref this.periodsViewModel, value);
        }

        private BestSolutionViewModel? bestSolutionViewModel;
        public BestSolutionViewModel? BestSolutionViewModel
        {
            get => this.bestSolutionViewModel;
            private set => Set(ref this.bestSolutionViewModel, value);
        }

        private RecoveryRateViewModel? recoveryRateViewModel;
        public RecoveryRateViewModel? RecoveryRateViewModel
        {
            get => this.recoveryRateViewModel;
            private set => Set(ref this.recoveryRateViewModel, value);
        }

        private ErrorViewModel? errorViewModel;
        public ErrorViewModel? ErrorViewModel
        {
            get => this.errorViewModel;
            private set => Set(ref this.errorViewModel, value);
        }

        private NeighborhoodDiversityViewModel? neighborhoodDiversityViewModel;
        public NeighborhoodDiversityViewModel? NeighborhoodDiversityViewModel
        {
            get => this.neighborhoodDiversityViewModel;
            private set => Set(ref this.neighborhoodDiversityViewModel, value);
        }

        private NeighborhoodFitnessViewModel? neighborhoodFitnessViewModel;
        public NeighborhoodFitnessViewModel? NeighborhoodFitnessViewModel
        {
            get => this.neighborhoodFitnessViewModel;
            private set => Set(ref this.neighborhoodFitnessViewModel, value);
        }

        private NeighborhoodsDiversityHeatMapViewModel? neighborhoodsDiversityHeatMapViewModel;
        public NeighborhoodsDiversityHeatMapViewModel? NeighborhoodsDiversityHeatMapViewModel
        {
            get => this.neighborhoodsDiversityHeatMapViewModel;
            private set => Set(ref this.neighborhoodsDiversityHeatMapViewModel, value);
        }

        private AgeLayerFitnessViewModel? ageLayerFitnessViewModel;
        public AgeLayerFitnessViewModel? AgeLayerFitnessViewModel
        {
            get => this.ageLayerFitnessViewModel;
            private set => Set(ref this.ageLayerFitnessViewModel, value);
        }

        private AgeHistogramViewModel? ageHistogramViewModel;
        public AgeHistogramViewModel? AgeHistogramViewModel
        {
            get => this.ageHistogramViewModel;
            private set => Set(ref this.ageHistogramViewModel, value);
        }

        private AgeViewModel? ageViewModel;
        public AgeViewModel? AgeViewModel
        {
            get => this.ageViewModel;
            private set => Set(ref this.ageViewModel, value);
        }

        private LayerDiversityViewModel? layerDiversityViewModel;
        public LayerDiversityViewModel? LayerDiversityViewModel
        {
            get => this.layerDiversityViewModel;
            private set => Set(ref this.layerDiversityViewModel, value);
        }

        private LayersDiversityHeatMapViewModel? layersDiversityHeatMapViewModel;
        public LayersDiversityHeatMapViewModel? LayersDiversityHeatMapViewModel
        {
            get => this.layersDiversityHeatMapViewModel;
            private set => Set(ref this.layersDiversityHeatMapViewModel, value);
        }

        public RunsViewModel(RunModel runModel)
        {
            RunModel = runModel;
        }

        private void UpdateSelectedRun()
        {
            if(selectedSolution == null)
            {
                AlgorithmParametersViewModel = null;
                ResultTourViewModel = null;
                FitnessPerGenerationViewModel = null;
                DiversityPerGenerationViewModel = null;
                DiversityHeatMapViewModel = null;
                EdgeFrequenciesViewModel = null;
                EdgeFrequenciesBestKnownViewModel = null;
                PeriodsViewModel = null;
                BestSolutionViewModel = null;
                RecoveryRateViewModel = null;
                ErrorViewModel = null;
                NeighborhoodDiversityViewModel = null;
                NeighborhoodFitnessViewModel = null;
                NeighborhoodsDiversityHeatMapViewModel = null;
                AgeLayerFitnessViewModel = null;
                AgeHistogramViewModel = null;
                AgeViewModel = null;
                LayerDiversityViewModel = null;
                LayersDiversityHeatMapViewModel = null;
            }
            else
            {
                AlgorithmParametersViewModel = new AlgorithmParametersViewModel(selectedSolution);
                ResultTourViewModel = new ResultTourViewModel(selectedSolution);
                FitnessPerGenerationViewModel = new FitnessPerGenerationViewModel(selectedSolution);
                DiversityPerGenerationViewModel = new DiversityPerGenerationViewModel(selectedSolution);
                DiversityHeatMapViewModel = new DiversityHeatMapViewModel(selectedSolution);
                EdgeFrequenciesViewModel = new EdgeFrequenciesViewModel(selectedSolution);
                EdgeFrequenciesBestKnownViewModel = new EdgeFrequenciesBestKnownViewModel(selectedSolution);
                PeriodsViewModel = new PeriodsViewModel(selectedSolution);
                BestSolutionViewModel = new BestSolutionViewModel(selectedSolution);
                RecoveryRateViewModel = new RecoveryRateViewModel(selectedSolution);
                ErrorViewModel = new ErrorViewModel(selectedSolution);

                if(selectedSolution.AlgorithmReport.Configuration is NeighborhoodGeneticAlgorithmConfiguration)
                {
                    NeighborhoodDiversityViewModel = new NeighborhoodDiversityViewModel(selectedSolution);
                    NeighborhoodFitnessViewModel = new NeighborhoodFitnessViewModel(selectedSolution);
                    NeighborhoodsDiversityHeatMapViewModel = new NeighborhoodsDiversityHeatMapViewModel(selectedSolution);
                }

                if(selectedSolution.AlgorithmReport.Configuration is AlpsGeneticAlgorithmConfiguration)
                {
                    AgeLayerFitnessViewModel = new AgeLayerFitnessViewModel(selectedSolution);
                    AgeHistogramViewModel = new AgeHistogramViewModel(selectedSolution);
                    AgeViewModel = new AgeViewModel(selectedSolution);
                    LayerDiversityViewModel = new LayerDiversityViewModel(selectedSolution);
                    LayersDiversityHeatMapViewModel = new LayersDiversityHeatMapViewModel(selectedSolution);
                }
            }
        }
    }
}
