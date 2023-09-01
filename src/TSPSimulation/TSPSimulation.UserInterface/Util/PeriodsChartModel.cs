using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TSPSimulation.Reporting;

namespace TSPSimulation.UserInterface.Util
{
    public class PeriodsChartModel : PlotModel
    {
        public PeriodsChartModel(IList<PeriodReport> periods, IList<GenerationReport> generations, double max, bool showOptimum = true)
        {
            BuildModel(periods, generations, max, showOptimum);
        }

        private void BuildModel(IList<PeriodReport> periods, IList<GenerationReport> generations, double max, bool showOptimum)
        {
            LinearBarSeries barSeries = new LinearBarSeries { FillColor = OxyColors.LightGray, BarWidth = 2 };
            for (int i = 1; i < periods.Count; i++)
            {
                barSeries.Points.Add(new DataPoint(periods[i].StartGeneration, max));
            }
            this.Series.Add(barSeries);

            if (showOptimum)
            {
                LineSeries optimalFitnessSeries = new LineSeries();
                foreach (var generation in generations)
                {
                    optimalFitnessSeries.Points.Add(new DataPoint(generation.Generation, generation.Period.Problem.OptimalSolution.Fitness));
                }
                this.Series.Add(optimalFitnessSeries);
            }
        }

    }
}
