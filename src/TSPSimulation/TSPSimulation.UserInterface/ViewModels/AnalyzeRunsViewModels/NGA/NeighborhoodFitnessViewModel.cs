using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using TSPSimulation.Algorithms;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.NGA
{
    public class NeighborhoodFitnessViewModel
    {
        public PlotModel? FitnessChartModel { get; private set; }

        private AlgorithmReport algorithmReport;

        public NeighborhoodFitnessViewModel(DynamicTSPSolution solution) { 
            algorithmReport = solution.AlgorithmReport;
            double? maxFitnessValue = algorithmReport.GetGenerationReports().Max(gr => gr.WorstFitnessInPeriod);
            if (maxFitnessValue.HasValue)
            {
                FitnessChartModel = new PeriodsChartModel(algorithmReport.Periods, algorithmReport.GetGenerationReports(), maxFitnessValue.Value);
                AddFitnessSeries();
            }
        }

        public void AddFitnessSeries()
        {
            if(algorithmReport.Configuration is NeighborhoodGeneticAlgorithmConfiguration ngaConfig)
            {
                IList<GenerationReport> generations = algorithmReport.GetGenerationReports();
                int neighborhoods = (int) Math.Ceiling((double) ngaConfig.PopulationSize / ngaConfig.NeighborhoodSize);
                if(ngaConfig.NeighborhoodSizeMin.HasValue)
                {
                    neighborhoods = (int)Math.Ceiling((double)ngaConfig.PopulationSize / ngaConfig.NeighborhoodSizeMin.Value);
                }
                for (int i = 0; i < neighborhoods; i++)
                {
                    LineSeries bestFitnessSeries = new LineSeries { Title = $"Best N{i}" };
                    LineSeries averageFitnessSeries = new LineSeries { Title = $"Average N{i}" };
                    LineSeries worstFitnessSeries = new LineSeries { Title = $"Worst N{i}" };
                    foreach(var generation in generations)
                    {
                        if(i < generation.NeighborhoodBests.Count)
                        {
                            bestFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.NeighborhoodBests[i]));
                            averageFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.NeighborhoodAverages[i]));
                            worstFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.NeighborhoodWorsts[i]));
                        }
                    }
                    FitnessChartModel.Series.Add(bestFitnessSeries);
                    FitnessChartModel.Series.Add(averageFitnessSeries);
                    FitnessChartModel.Series.Add(worstFitnessSeries);
                }
                FitnessChartModel.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });
            }
        }
    }
}
