using OxyPlot.Series;
using OxyPlot;
using TSPSimulation.Algorithms;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using OxyPlot.Legends;
using System.Collections.Generic;
using System;
using TSPSimulation.Algorithms.Configuration;
using OxyPlot.Axes;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.NGA
{
    public class NeighborhoodDiversityViewModel
    {
        public PlotModel DiversityChartModel { get; private set; }
        private AlgorithmReport algorithmReport;

        public NeighborhoodDiversityViewModel(DynamicTSPSolution solution)
        {
            this.algorithmReport = solution.AlgorithmReport;
            DiversityChartModel = new PlotModel();
            BuildChart();
        }

        private void BuildChart()
        {
            if (algorithmReport.Configuration is NeighborhoodGeneticAlgorithmConfiguration ngaConfig)
            {
                IEnumerable<GenerationReport> generations = algorithmReport.GenerationReportsWithDiversity();
                int neighborhoods = (int)Math.Ceiling((double)ngaConfig.PopulationSize / ngaConfig.NeighborhoodSize);
                if (ngaConfig.NeighborhoodSizeMin.HasValue)
                {
                    neighborhoods = (int)Math.Ceiling((double)ngaConfig.PopulationSize / ngaConfig.NeighborhoodSizeMin.Value);
                }
                for (int i = 0; i < neighborhoods; i++)
                {
                    BarSeries diversitySeries = new BarSeries { Title = $"N{i}" };
                    foreach (var generation in generations)
                    {
                        if(i < generation.NeighborhoodBests.Count)
                        {
                            diversitySeries.Items.Add(new BarItem(generation.NeighborhoodDiversities[i]));
                        }
                    }
                    DiversityChartModel.Series.Add(diversitySeries);
                }
                CategoryAxis generationsAxis = new CategoryAxis { Position = AxisPosition.Left };
                int cnt = 0;
                foreach (var generation in generations)
                {
                    generationsAxis.Labels.Add($"Period {++cnt}");
                }
                DiversityChartModel.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });
                DiversityChartModel.Axes.Add(generationsAxis);
            }
        }
    }
}
