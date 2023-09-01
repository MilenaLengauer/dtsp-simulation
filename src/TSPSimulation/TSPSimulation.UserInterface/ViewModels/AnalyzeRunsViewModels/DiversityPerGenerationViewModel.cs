using OxyPlot.Series;
using OxyPlot;
using TSPSimulation.Algorithms;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using OxyPlot.Axes;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using TSPSimulation.UserInterface.Models;
using System.Windows.Input;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class DiversityPerGenerationViewModel
    {
        public PlotModel DiversityChartModel { get; private set; }
        private AlgorithmReport algorithmReport;
        private ChartPrinter chartPrinter = new ChartPrinter();

        public ICommand ExportCommand { get; }

        public DiversityPerGenerationViewModel(DynamicTSPSolution solution)
        {
            this.algorithmReport = solution.AlgorithmReport;

            ExportCommand = new AsyncDelegateCommand(PrintToPdf, _ => true);

            DiversityChartModel = new PlotModel();
            BuildChart();
        }

        private void BuildChart()
        {
            LinearBarSeries diversitySeries = new();
            var generations = algorithmReport.GetGenerationReports();
            for (int i = 0; i < generations.Count; i++)
            {
                GenerationReport report = generations[i];
                if(report.PopulationDiversity != null)
                {
                    diversitySeries.Points.Add(new DataPoint(report.Generation, report.PopulationDiversity.Value));
                }
            }
            DiversityChartModel.Series.Add(diversitySeries);
            DiversityChartModel.Axes.Add(new LinearAxis { 
                Position = AxisPosition.Bottom,
                Title = "Generation"
            });

            DiversityChartModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Diversity",
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColors.LightGray,
                Minimum = 0,
                Maximum = 1
            });
            DiversityChartModel.DefaultFontSize = 14;
        }

        public async Task PrintToPdf(object _)
        {
            chartPrinter.PrintChart(DiversityChartModel, 600, 200);
        }
    }
}
