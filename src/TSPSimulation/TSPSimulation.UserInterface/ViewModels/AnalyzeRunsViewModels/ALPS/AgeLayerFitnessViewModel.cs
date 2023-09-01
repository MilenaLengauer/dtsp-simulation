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
using TSPSimulation.UserInterface.Util;
using OxyPlot.Axes;
using System.Windows.Input;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.ALPS
{
    public class AgeLayerFitnessViewModel
    {
        public PlotModel? FitnessChartModel { get; private set; }

        private AlgorithmReport algorithmReport;

        private ChartPrinter chartPrinter = new ChartPrinter();
        public ICommand ExportCommand { get; }

        private double? maxFitnessValue;

        public AgeLayerFitnessViewModel(DynamicTSPSolution solution)
        {
            algorithmReport = solution.AlgorithmReport;

            ExportCommand = new AsyncDelegateCommand(PrintToPdf, _ => true);

            maxFitnessValue = algorithmReport.GetGenerationReports().Max(gr => gr.WorstFitnessInPeriod);
            if (maxFitnessValue.HasValue)
            {
                FitnessChartModel = new PeriodsChartModel(algorithmReport.Periods, algorithmReport.GetGenerationReports(), maxFitnessValue.Value);
                AddFitnessSeries();
            }
        }

        public void AddFitnessSeries()
        {
            if (algorithmReport.Configuration is AlpsGeneticAlgorithmConfiguration alpsConfig)
            {
                IList<GenerationReport> generations = algorithmReport.GetGenerationReports();
                int layers = alpsConfig.NumberOfLayers;
                for (int i = 0; i < layers; i++)
                {
                    LineSeries bestFitnessSeries = new LineSeries { Title = $"Best L{i}" };
                    LineSeries averageFitnessSeries = new LineSeries { Title = $"Average L{i}" };
                    LineSeries worstFitnessSeries = new LineSeries { Title = $"Worst L{i}" };
                    foreach (var generation in generations)
                    {
                        if (i < generation.AgeLayerBests.Count)
                        {
                            bestFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.AgeLayerBests[i]));
                            averageFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.AgeLayerAverages[i]));
                            worstFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.AgeLayerWorsts[i]));
                        }
                    }
                    FitnessChartModel.Series.Add(bestFitnessSeries);
                    FitnessChartModel.Series.Add(averageFitnessSeries);
                    FitnessChartModel.Series.Add(worstFitnessSeries);
                }

                FitnessChartModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Generation"
                });

                FitnessChartModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Fitness"
                });

                FitnessChartModel.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });
                FitnessChartModel.DefaultFontSize = 14;
            }
        }

        public async Task PrintToPdf(object _)
        {
            if(FitnessChartModel != null)
            {
                /*var model = new PeriodsChartModel(algorithmReport.Periods, algorithmReport.GetGenerationReports(), maxFitnessValue.Value);
                var visibleSeries = FitnessChartModel.Series.Where(s => s.IsVisible);
                foreach(LineSeries series in visibleSeries)
                {
                    var seriesCopy = new LineSeries { Title = series.Title };
                    foreach(var point in series.Points)
                    {
                        seriesCopy.Points.Add(new DataPoint(point.X, point.Y));
                    }
                    model.Series.Add(seriesCopy);
                }
                model.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Generation"
                });

                model.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Fitness"
                });
                model.DefaultFontSize = 14;*/

                FitnessChartModel.Legends[0].IsLegendVisible = false;
                chartPrinter.PrintChart(FitnessChartModel, 600, 300);
                FitnessChartModel.Legends[0].IsLegendVisible = true;
            }
        }
    }
}
