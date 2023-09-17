using OperationsResearch;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.CompareRunsViewModels
{
    public class GenerationMetricsViewModel : NotifyPropertyChanged
    {
        private GenerationMetric? selectedMetric;
        public GenerationMetric? SelectedMetric
        {
            get => selectedMetric;
            set
            {
                selectedMetric = value;
                DrawChart();
            }
        }
        public Array AvailableMetrics { get; }

        private PlotModel? chart;
        public PlotModel? Chart
        {
            get => chart;
            private set => Set(ref chart, value);
        }

        private bool showGrouped;
        public bool ShowGrouped 
        { 
            get => showGrouped;
            set
            {
                showGrouped = value;
                DrawChart();
            }
        }

        private string stepSize = "1000";
        public string StepSize
        {
            get => stepSize;
            set
            {
                stepSize = value;
                DrawChart();
            }
        }

        private RunModel runModel;

        public GenerationMetricsViewModel(RunModel model)
        {
            runModel = model;
            AvailableMetrics = Enum.GetValues(typeof(GenerationMetric));
        }

        public void DrawChart()
        {
            if (SelectedMetric == null) return;

            if(ShowGrouped)
            {
                DrawChartGrouped();
            } else
            {
                DrawChartSingle();
            }
        }

        public void DrawChartSingle()
        {
            IEnumerable<IGrouping<int, DynamicTSPSolution>> grouped = runModel.Solutions.GroupBy(s => s.AlgorithmReport.Configuration.Group);

            PlotModel model = new PlotModel();
            int i = 0;
            foreach (var group in grouped)
            {
                foreach(var solution in group)
                {
                    LineSeries series = new LineSeries { Title = $"{solution.Name}", Color = model.DefaultColors[i], StrokeThickness = 1 };

                    IEnumerable<(int, double)> data = new List<(int, double)>();
                    switch (SelectedMetric)
                    {
                        case GenerationMetric.Error:
                            data = solution.AlgorithmReport.GetErrorsInPercent();
                            break;
                        case GenerationMetric.Fitness:
                            data = solution.AlgorithmReport.GetGenerationReports().Select(g => (g.Evaluations, g.BestFitnessInPeriod));
                            break;
                        case GenerationMetric.Accuracy:
                            data = solution.AlgorithmReport.GetAccuracies();
                            break;
                    }
                    
                    foreach((int x, double y) in data)
                    {
                        series.Points.Add(new DataPoint(x, y));
                    }

                    model.Series.Add(series);
                }
                i++;
            }
            model.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });

            Chart = model;
        }

        public void DrawChartGrouped()
        {
            int stepSizeNum;
            bool success = Int32.TryParse(StepSize, out stepSizeNum);
            if (!success) return;

            IEnumerable<IGrouping<int, DynamicTSPSolution>> grouped = runModel.Solutions.GroupBy(s => s.AlgorithmReport.Configuration.Group);

            PlotModel model = new PlotModel();
            int i = 0;
            foreach (var group in grouped)
            {
                IEnumerable<(int x, double y)> data = new List<(int, double)>();

                switch (SelectedMetric)
                {
                    case GenerationMetric.Error:
                        data = group.SelectMany(s => s.AlgorithmReport.GetErrorsInPercent());
                        break;
                    case GenerationMetric.Fitness:
                        data = group.SelectMany(s => s.AlgorithmReport.GetGenerationReports().Select(g => (g.Evaluations, g.BestFitnessInPeriod)));
                        break;
                }

                IList<(int x, double y)> dataSorted = data.OrderBy(p => p.x).ToList();

                IList<(int x, double avg)> averageData = new List<(int, double)>();
                IList<(int x, double y)> dataGroup = new List<(int, double)>();
                int limit = stepSizeNum;
                for (int j = 0; j < dataSorted.Count; j++)
                {
                    int x = dataSorted[j].x;
                    double y = dataSorted[j].y;
                    if(x <= limit)
                    {
                        dataGroup.Add((x, y));
                    } else
                    {
                        if(dataGroup.Count > 0)
                        {
                            double avg = dataGroup.Average(p => p.y);
                            averageData.Add((limit, avg));
                        }
                        limit += stepSizeNum;
                        dataGroup = new List<(int, double)>();
                        j--;
                    }
                }


                //var dataPoints = data.GroupBy(p => p.Item1).Select(g => new { X = g.Key, Y = g.Average(v => v.Item2) }).OrderBy(o => o.X);

                LineSeries series = new LineSeries { Title = $"Group {group.Key}", Color = model.DefaultColors[i], StrokeThickness = 0.5 };

                foreach (var point in averageData)
                {
                    series.Points.Add(new DataPoint(point.x, point.avg));
                }

                model.Series.Add(series);
                i++;
            }
            model.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });

            Chart = model;
        }

    }
}
