using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.ProblemDefinition.Static;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class PeriodsViewModel : NotifyPropertyChanged
    {
        public IList<PeriodReport> Periods { get; set; }

        private PeriodReport selectedPeriod;
        public PeriodReport SelectedPeriod
        {
            get => this.selectedPeriod;
            set
            {
                this.selectedPeriod = value;
                BuildChart();
            }
        }
        public double Error { get; private set; }
        public double ErrorInitializingExcluded { get; private set; }

        public double Stability { get; private set; }

        private PlotModel? model;
        public PlotModel? Model
        {
            get => this.model;
            set
            {
                Set(ref this.model, value);
            }
        }


        private bool optimalPathVisible = true;
        public bool OptimalPathVisible
        {
            get => optimalPathVisible;
            set
            {
                this.optimalPathVisible = value;
                optimalPathSeries.IsVisible = this.optimalPathVisible;
                Model?.InvalidatePlot(true);
            }
        }

        private bool reachedPathVisible = true;
        public bool ReachedPathVisible
        {
            get => reachedPathVisible;
            set
            {
                this.reachedPathVisible = value;
                reachedPathSeries.IsVisible = this.reachedPathVisible;
                Model?.InvalidatePlot(true);
            }
        }

        private LineSeries optimalPathSeries;
        private LineSeries reachedPathSeries;

        public PeriodsViewModel(DynamicTSPSolution solution)
        {
            Error = solution.AlgorithmReport.ErrorInPercent;
            ErrorInitializingExcluded = solution.AlgorithmReport.ErrorInPercentInitializingExcluded;
            Stability = solution.AlgorithmReport.Stability;
            Periods = solution.AlgorithmReport.Periods;
            SelectedPeriod = Periods.First();
        }

        private void BuildChart()
        {
            TSP tsp = SelectedPeriod.Problem;
            var model = new PlotModel();
            ScatterSeries scatterSeries = new ScatterSeries();
            for (int i = 0; i < tsp.Coordinates.Length; i++)
            {
                ScatterPoint scatterPoint = new ScatterPoint(
                    tsp.Coordinates[i].X,
                    tsp.Coordinates[i].Y,
                    3);
                if (i == tsp.Start || i == tsp.End)
                {
                    scatterPoint.Value = 0;
                }
                else if (tsp.Cities.Contains(i))
                {
                    scatterPoint.Value = 2;
                }
                else
                {
                    scatterPoint.Value = 1;
                }
                scatterSeries.Points.Add(scatterPoint);
            }
            model.Series.Add(scatterSeries);

            if(tsp.OptimalSolution != null)
            {
                optimalPathSeries = DrawPath(tsp.OptimalSolution, OptimalPathVisible);
                model.Series.Add(optimalPathSeries);
            }
            reachedPathSeries = DrawPath(SelectedPeriod.FinalSolution, ReachedPathVisible);
            model.Series.Add(reachedPathSeries);

            model.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.None,
                Palette = new OxyPalette(OxyColors.Blue, OxyColors.Gray, OxyColors.Red)
            });
            Model = model;
        }

        private LineSeries DrawPath(TSPSolution solution, bool isVisible)
        {
            var tsp = solution.Problem;
            LineSeries lineSeries = new LineSeries();
            lineSeries.IsVisible = isVisible;
            lineSeries.Points.Add(new DataPoint(tsp.Coordinates[tsp.Start].X, tsp.Coordinates[tsp.Start].Y));
            foreach (int city in solution.Path)
            {
                lineSeries.Points.Add(new DataPoint(tsp.Coordinates[city].X, tsp.Coordinates[city].Y));
            }
            lineSeries.Points.Add(new DataPoint(tsp.Coordinates[tsp.End].X, tsp.Coordinates[tsp.End].Y));
            return lineSeries;
        }

    }
}
