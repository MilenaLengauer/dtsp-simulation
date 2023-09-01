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

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class ErrorViewModel
    {
        public PlotModel? Model { get; private set; }
        public double Error { get; private set; }
        public double ErrorInitializingExcluded { get; private set; }

        private AlgorithmReport algorithmReport;

        public ErrorViewModel(DynamicTSPSolution solution)
        {
            algorithmReport = solution.AlgorithmReport;
            Error = algorithmReport.ErrorInPercent * 100;
            ErrorInitializingExcluded = algorithmReport.ErrorInPercentInitializingExcluded * 100;

            double? maxValue = algorithmReport.GetErrorsInPercent().Max(e => e.error);
            if (maxValue.HasValue)
            {
                Model = new PeriodsChartModel(algorithmReport.Periods, algorithmReport.GetGenerationReports(), maxValue.Value * 1.1, false);
                AddErrorSeries();
            }
        }

        public void AddErrorSeries()
        {
            LineSeries series = new LineSeries();
            var erros = algorithmReport.GetErrorsInPercent();
            foreach (var error in erros)
            {
                series.Points.Add(new DataPoint(error.evaluations, error.error));
            }
            Model.Series.Add(series);
        }
    }
}
