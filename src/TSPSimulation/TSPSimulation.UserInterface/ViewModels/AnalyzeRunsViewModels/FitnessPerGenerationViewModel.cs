using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;
using TSPSimulation.Algorithms;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class FitnessPerGenerationViewModel
    {
        public PlotModel? FitnessChartModel { get; private set; }

        private AlgorithmReport algorithmReport;

        public FitnessPerGenerationViewModel(DynamicTSPSolution solution) { 
            algorithmReport = solution.AlgorithmReport;
            double? maxFitnessValue = algorithmReport.GetGenerationReports().MaxBy(gr => gr.BestFitnessInPeriod)?.BestFitnessInPeriod;
            if (maxFitnessValue.HasValue)
            {
                FitnessChartModel = new PeriodsChartModel(algorithmReport.Periods, algorithmReport.GetGenerationReports(), maxFitnessValue.Value * 1.1);
                AddFitnessSeries();
            }
        }

        public void AddFitnessSeries()
        {
            LineSeries bestFitnessSeries = new LineSeries { Title = "Best" };
            LineSeries averageFitnessSeries = new LineSeries { Title = "Average" };
            LineSeries worstFitnessSeries = new LineSeries { Title = "Worst" };
            IList<GenerationReport> generations = algorithmReport.GetGenerationReports();
            foreach(var generation in generations)
            {
                bestFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.BestFitnessInPeriod));
                averageFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.AverageFitnessInPeriod));
                worstFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.WorstFitnessInPeriod));
            }
            FitnessChartModel.Series.Add(bestFitnessSeries);
            FitnessChartModel.Series.Add(averageFitnessSeries);
            FitnessChartModel.Series.Add(worstFitnessSeries);

            FitnessChartModel.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });
        }
    }
}
