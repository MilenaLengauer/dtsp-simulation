using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class EdgeFrequenciesViewModel : NotifyPropertyChanged
    {
        public PlotModel EdgeCountModel { get; private set; }

        private bool uniqueEdgesSeriesVisible = true;
        public bool UniqueEdgesSeriesVisible { 
            get => this.uniqueEdgesSeriesVisible;
            set
            {
                this.uniqueEdgesSeriesVisible = value;
                uniqueEdgesSeries.IsVisible = this.uniqueEdgesSeriesVisible;
                EdgeCountModel.InvalidatePlot(true);
            }
        }
        private bool fixedEdgesSeriesVisible = true;
        public bool FixedEdgesSeriesVisible
        {
            get => fixedEdgesSeriesVisible;
            set
            {
                fixedEdgesSeriesVisible = value;
                fixedEdgesSeries.IsVisible = fixedEdgesSeriesVisible;
                EdgeCountModel.InvalidatePlot(true);
            }
        }
        private bool fixedEdgesOfBestSeriesVisible = true;
        public bool FixedEdgesOfBestSeriesVisible
        {
            get => fixedEdgesOfBestSeriesVisible;
            set
            {
                fixedEdgesOfBestSeriesVisible = value;
                fixedEdgesOfBestSeries.IsVisible = fixedEdgesOfBestSeriesVisible;
                EdgeCountModel.InvalidatePlot(true);
            }
        }
        private bool lostEdgesOfBestSeriesVisible = true;
        public bool LostEdgesOfBestSeriesVisible
        {
            get => lostEdgesOfBestSeriesVisible;
            set
            {
                this.lostEdgesOfBestSeriesVisible = value;
                lostEdgesOfBestSeries.IsVisible = this.lostEdgesOfBestSeriesVisible;
                EdgeCountModel.InvalidatePlot(true);
            }
        }
        private bool averageContainedBestKnownEdgesSeriesVisible = true;
        public bool AverageContainedBestKnownEdgesSeriesVisible
        {
            get => averageContainedBestKnownEdgesSeriesVisible;
            set
            {
                averageContainedBestKnownEdgesSeriesVisible = value;
                averageContainedBestKnownEdgesSeries.IsVisible = averageContainedBestKnownEdgesSeriesVisible;
                EdgeCountModel.InvalidatePlot(true);
            }
        }

        private DynamicTSPSolution solution;
        private BarSeries uniqueEdgesSeries;
        private BarSeries fixedEdgesSeries;
        private BarSeries fixedEdgesOfBestSeries;
        private BarSeries lostEdgesOfBestSeries;
        private BarSeries averageContainedBestKnownEdgesSeries;

        public EdgeFrequenciesViewModel(DynamicTSPSolution solution)
        {
            this.solution = solution;
            EdgeCountModel = new PlotModel();

            BuildEdgeCountLineChart();
        }

        private void BuildEdgeCountLineChart()
        {
            string axisAll = "All";
            string axisBestKnown = "BestKnown";
            EdgeCountModel.Axes.Add(new LinearAxis { Key = axisAll, Position = AxisPosition.Top, Minimum = 0, AbsoluteMinimum = 0 }) ;
            EdgeCountModel.Axes.Add(new LinearAxis { Key = axisBestKnown, Position = AxisPosition.Bottom, Minimum = 0, AbsoluteMinimum = 0 });

            uniqueEdgesSeries = new BarSeries { Title = "Unique Edges", XAxisKey = axisAll };
            fixedEdgesSeries = new BarSeries { Title = "Fixed Edges", XAxisKey = axisBestKnown };
            fixedEdgesOfBestSeries = new BarSeries { Title = "Fixed Edges of Best", XAxisKey = axisBestKnown };
            lostEdgesOfBestSeries = new BarSeries { Title = "Lost Edges of Best", XAxisKey = axisBestKnown };
            averageContainedBestKnownEdgesSeries = new BarSeries { Title = "Avg. Contained Best Edges", XAxisKey = axisBestKnown };

            CategoryAxis generationsAxis = new CategoryAxis { Position = AxisPosition.Left };
            int i = 0;

            foreach (var generationReport in solution.AlgorithmReport.GenerationReportsWithEdgeFrequencies())
            {
                uniqueEdgesSeries.Items.Add(new BarItem(generationReport.UniqueEdgesCount.Value));
                fixedEdgesSeries.Items.Add(new BarItem(generationReport.FixedEdgesCount.Value));
                fixedEdgesOfBestSeries.Items.Add(new BarItem(generationReport.FixedEdgesOfBestKnownCount.Value));
                lostEdgesOfBestSeries.Items.Add(new BarItem(generationReport.LostEdgesOfBestKnownCount.Value));
                averageContainedBestKnownEdgesSeries.Items.Add(new BarItem(generationReport.AverageContainedBestKnownEdges.Value));

                generationsAxis.Labels.Add($"Period {++i}");
            }
            EdgeCountModel.Series.Add(uniqueEdgesSeries);
            EdgeCountModel.Series.Add(fixedEdgesSeries);
            EdgeCountModel.Series.Add(fixedEdgesOfBestSeries);
            EdgeCountModel.Series.Add(lostEdgesOfBestSeries);
            EdgeCountModel.Series.Add(averageContainedBestKnownEdgesSeries);

            EdgeCountModel.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight, IsLegendVisible = true });
            EdgeCountModel.Axes.Add(generationsAxis);
        }
    }
}
