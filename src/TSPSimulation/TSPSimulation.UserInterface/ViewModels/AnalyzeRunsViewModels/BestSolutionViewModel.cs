using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SimSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.ProblemDefinition.Static;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class BestSolutionViewModel : NotifyPropertyChanged
    {
        private PlotModel? mapModel;
        public PlotModel? MapModel
        {
            get => this.mapModel;
            private set => Set(ref this.mapModel, value);
        }

        private Dictionary<int, PlotModel> charts = new Dictionary<int, PlotModel>();

        private int sliderValue = 0;
        public int SliderValue
        {
            get => this.sliderValue;
            set
            {
                Set(ref this.sliderValue, value);
                BuildChart(this.sliderValue);
            }
        }

        public int SliderMin { get; set; }
        public int SliderMax { get; set; }
        public int SliderStepSize { get; set; } = 1;

        private DynamicTSPSolution solution;

        public BestSolutionViewModel(DynamicTSPSolution solution)
        {
            this.solution = solution;

            SliderMin = 0;
            SliderMax = solution.AlgorithmReport.GetGenerationReports().LastOrDefault()?.Generation ?? 0;
            BuildChart(0);
        }

        private void BuildChart(int generation)
        {
            if (charts.ContainsKey(generation))
            {
                MapModel = charts[generation];
                return;
            }
            

            var generationReport = solution.AlgorithmReport.GetGenerationReport(generation);
            if (generationReport == null) return;

            TSP tsp = generationReport.Period.Problem;

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

            DrawPath(generationReport.BestSolution, tsp, model);

            model.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.None,
                Palette = new OxyPalette(OxyColors.Blue, OxyColors.Gray, OxyColors.Red)
            });

            MapModel = model;
            charts[generation] = model;
        }



        private void DrawPath(TSPSolution solution, TSP tsp, PlotModel model)
        {
            LineSeries lineSeries = new LineSeries();
            lineSeries.Points.Add(new DataPoint(tsp.Coordinates[tsp.Start].X, tsp.Coordinates[tsp.Start].Y));
            foreach (int city in solution.Path)
            {
                lineSeries.Points.Add(new DataPoint(tsp.Coordinates[city].X, tsp.Coordinates[city].Y));
            }
            lineSeries.Points.Add(new DataPoint(tsp.Coordinates[tsp.End].X, tsp.Coordinates[tsp.End].Y));
            model.Series.Add(lineSeries);
        }
    }
}
