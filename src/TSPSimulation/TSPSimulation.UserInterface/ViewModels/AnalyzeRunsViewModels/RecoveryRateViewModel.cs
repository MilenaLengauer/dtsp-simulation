using Docker.DotNet.Models;
using OperationsResearch;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class RecoveryRateViewModel : NotifyPropertyChanged
    {
        public IList<PeriodReport> Periods { get; set; }

        private PeriodReport selectedPeriod;
        public PeriodReport SelectedPeriod
        {
            get => this.selectedPeriod;
            set
            {
                HighlightSelectedPoint(this.selectedPeriod, value); 
                this.selectedPeriod = value;
            }
        }

        private PlotModel? model;
        public PlotModel? Model
        {
            get => this.model;
            set
            {
                Set(ref this.model, value);
            }
        }

        private Dictionary<int, ScatterPoint> points = new Dictionary<int, ScatterPoint>();

        public RecoveryRateViewModel(DynamicTSPSolution solution)
        {
            Periods = solution.AlgorithmReport.Periods;
            BuildChart();
            SelectedPeriod = Periods.First();
        }

        private void BuildChart()
        {
            var model = new PlotModel();

            model.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.None,
                Palette = new OxyPalette(OxyColors.Blue, OxyColors.Red)
            });

            Func<double, double> function = (x) => x;
            model.Series.Add(new FunctionSeries(function, 0, 1, 0.1));

            var scatterSeries = new ScatterSeries();
            foreach (var period in Periods)
            {
                var point = new ScatterPoint(period.RecoveryRate, period.AbsoluteRecoveryRate);
                point.Value = 0;
                points.Add(period.PeriodNumber, point);
                scatterSeries.Points.Add(point);
            }
            model.Series.Add(scatterSeries);

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            Model = model;
        }

        private void HighlightSelectedPoint(PeriodReport previousSelection, PeriodReport newSelection)
        {
            if (previousSelection != null) points[previousSelection.PeriodNumber].Value = 0;
            if (newSelection != null) points[newSelection.PeriodNumber].Value = 1;
            Model.InvalidatePlot(true);
        }
    }
}
