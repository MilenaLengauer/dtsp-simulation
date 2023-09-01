using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Util;
using OxyPlot.Axes;
using System.Windows.Input;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.ALPS
{
    public class AgeViewModel
    {
        public PlotModel? AgeChartModel { get; private set; }

        private AlgorithmReport algorithmReport;

        private ChartPrinter chartPrinter = new ChartPrinter();

        public ICommand ExportCommand { get; }

        public AgeViewModel(DynamicTSPSolution solution)
        {
            algorithmReport = solution.AlgorithmReport;

            ExportCommand = new AsyncDelegateCommand(PrintToPdf, _ => true);

            int? maxAge = algorithmReport.GetGenerationReports().Max(gr => gr.OldestAge);
            if(maxAge.HasValue)
            {
                AgeChartModel = new PeriodsChartModel(algorithmReport.Periods, algorithmReport.GetGenerationReports(), (double)maxAge, false);
                AddAgeSeries();
            }            
        }

        public void AddAgeSeries()
        {
            LineSeries youngestSeries = new LineSeries { Title = "Youngest" };
            LineSeries averageSeries = new LineSeries { Title = "Average" };
            LineSeries oldestSeries = new LineSeries { Title = "Oldest" };
            IList<GenerationReport> generations = algorithmReport.GetGenerationReports();
            foreach (var generation in generations)
            {
                if(generation.YoungestAge.HasValue && generation.OldestAge.HasValue && generation.AverageAge.HasValue)
                {
                    youngestSeries.Points.Add(new DataPoint(generation.Generation, generation.YoungestAge.Value));
                    averageSeries.Points.Add(new DataPoint(generation.Generation, generation.AverageAge.Value));
                    oldestSeries.Points.Add(new DataPoint(generation.Generation, generation.OldestAge.Value));
                }
                
            }
            AgeChartModel.Series.Add(youngestSeries);
            AgeChartModel.Series.Add(averageSeries);
            AgeChartModel.Series.Add(oldestSeries);

            AgeChartModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Generation"
            });

            AgeChartModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Age"
            });

            AgeChartModel.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });
            AgeChartModel.DefaultFontSize = 14;
        }

        public async Task PrintToPdf(object _)
        {
            chartPrinter.PrintChart(AgeChartModel, 600, 300);
        }
    }
}
