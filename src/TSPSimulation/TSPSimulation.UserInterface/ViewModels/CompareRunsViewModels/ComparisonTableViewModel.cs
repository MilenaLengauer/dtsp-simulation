using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.CompareRunsViewModels
{
    public class ComparisonTableViewModel : NotifyPropertyChanged
    {
        private RunMetric? selectedMetric;
        public RunMetric? SelectedMetric { 
            get => selectedMetric; 
            set
            {
                selectedMetric = value;
                LoadData();
            } 
        }
        public Array AvailableMetrics { get; }

        private IList<RunGroupAnalysis>? data;
        public IList<RunGroupAnalysis>? Data {
            get => data; 
            private set => Set(ref data, value);
        }

        private RunModel runModel;

        public ComparisonTableViewModel(RunModel model)
        {
            runModel = model;
            AvailableMetrics = Enum.GetValues(typeof(RunMetric));
        }

        public void LoadData()
        {
            IEnumerable<IGrouping<int, DynamicTSPSolution>> reportsGrouped = runModel.Solutions.GroupBy(s => s.AlgorithmReport.Configuration.Group);
            switch (SelectedMetric)
            {
                case RunMetric.Error:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.AlgorithmReport.ErrorInPercent).ToList())).ToList();
                    break;
                case RunMetric.ErrorInitializingExcluded:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.AlgorithmReport.ErrorInPercentInitializingExcluded).ToList())).ToList();
                    break;
                case RunMetric.FinalTourLength:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.GetFinalDistance()).ToList())).ToList();
                    break;
                case RunMetric.Stability:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.AlgorithmReport.Stability).ToList())).ToList();
                    break;
                case RunMetric.RecoveryRate:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.AlgorithmReport.RecoveryRate).ToList())).ToList();
                    break;
                case RunMetric.AbsoluteRecoveryRate:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.AlgorithmReport.AbsoluteRecoveryRate).ToList())).ToList();
                    break;
                case RunMetric.Accuracy:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.AlgorithmReport.Accuracy).ToList())).ToList();
                    break;
                case RunMetric.AccuracyInitializingExcluded:
                    Data = reportsGrouped.Select((g, i) => new RunGroupAnalysis($"Group{g.Key}", g.Select(s => s.AlgorithmReport.AccuracyInitializingExcluded).ToList())).ToList();
                    break;

            }
        }

    }
}
