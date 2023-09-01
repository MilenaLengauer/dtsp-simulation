using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Util;
using TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels;
using TSPSimulation.UserInterface.ViewModels.CompareRunsViewModels;

namespace TSPSimulation.UserInterface.ViewModels
{
    public class CompareRunsViewModel : NotifyPropertyChanged
    {
        public RunModel RunModel { get; private set; }

        private ComparisonTableViewModel? comparisonTableViewModel;
        public ComparisonTableViewModel? ComparisonTableViewModel
        {
            get => this.comparisonTableViewModel;
            private set => Set(ref this.comparisonTableViewModel, value);
        }

        private GenerationMetricsViewModel? generationMetricsViewModel;
        public GenerationMetricsViewModel? GenerationMetricsViewModel
        {
            get => this.generationMetricsViewModel;
            private set => Set(ref this.generationMetricsViewModel, value);
        }

        private CompareRunsViewModels.RecoveryRateViewModel? recoveryRateViewModel;
        public CompareRunsViewModels.RecoveryRateViewModel? RecoveryRateViewModel
        {
            get => this.recoveryRateViewModel;
            private set => Set(ref this.recoveryRateViewModel, value);
        }

        public CompareRunsViewModel(RunModel runModel)
        {
            RunModel = runModel;
            ComparisonTableViewModel = new ComparisonTableViewModel(runModel);
            GenerationMetricsViewModel = new GenerationMetricsViewModel(runModel);
            RecoveryRateViewModel = new CompareRunsViewModels.RecoveryRateViewModel(runModel);
        }
    }
}
