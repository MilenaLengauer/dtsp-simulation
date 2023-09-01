using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.ALPS
{
    public class LayerDiversityViewModel
    {
        public PlotModel DiversityChartModel { get; private set; }
        private readonly AlgorithmReport algorithmReport;

        public LayerDiversityViewModel(DynamicTSPSolution solution)
        {
            this.algorithmReport = solution.AlgorithmReport;
            DiversityChartModel = new PlotModel();
            BuildChart();
        }

        private void BuildChart()
        {
            if (algorithmReport.Configuration is AlpsGeneticAlgorithmConfiguration alpsConfig)
            {
                IEnumerable<GenerationReport> generations = algorithmReport.GenerationReportsWithDiversity();
                var layerStartAges = alpsConfig.LayerStartAges();

                for (int i = 0; i < layerStartAges.Count; i++)
                {
                    BarSeries diversitySeries = new BarSeries { Title = $"L{layerStartAges[i]}" };
                    foreach (var generation in generations)
                    {
                        if(i < generation.LayerDiversities.Count)
                        {
                            diversitySeries.Items.Add(new BarItem(generation.LayerDiversities[i]));
                        } else
                        {
                            diversitySeries.Items.Add(new BarItem(0));
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
