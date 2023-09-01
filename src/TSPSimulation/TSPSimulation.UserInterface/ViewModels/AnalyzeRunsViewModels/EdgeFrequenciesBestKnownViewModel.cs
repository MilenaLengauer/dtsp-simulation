using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels
{
    public class EdgeFrequenciesBestKnownViewModel : NotifyPropertyChanged
    {

        private PlotModel? bestKnownModel;
        public PlotModel? BestKnownModel
        {
            get => this.bestKnownModel;
            private set => Set(ref this.bestKnownModel, value);
        }

        private int sliderValue = 0;
        public int SliderValue
        {
            get => this.sliderValue;
            set
            {
                Set(ref this.sliderValue, value);
                BuildBestKnownModel(this.sliderValue);
            }
        }

        public int SliderMin { get; set; }
        public int SliderMax { get; set; }
        public int SliderStepSize { get; set; } = 1;

        private DynamicTSPSolution solution;

        public EdgeFrequenciesBestKnownViewModel(DynamicTSPSolution solution)
        {
            this.solution = solution;

            SliderMin = 0;
            SliderMax = solution.AlgorithmReport.GetGenerationReports().Count - 1;

            BuildBestKnownModel(0);
        }

        public void BuildBestKnownModel(int generation)
        {
            var model = new PlotModel();
            var generationReport = solution.AlgorithmReport.GetGenerationReport(generation);
            LinearBarSeries barSeries = new LinearBarSeries();

            int edgeIdx = 0;
            foreach(var edge in generationReport.Period.Problem.OptimalSolution.EdgesHashSet())
            {
                double? frequency = generationReport.EdgeFrequency(edge);
                if(frequency != null)
                {
                    barSeries.Points.Add(new DataPoint(edgeIdx++, frequency.Value));
                }
            }
            model.Series.Add(barSeries);

            model.Axes.Add(new LinearAxis
            {
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1,
                Minimum = 0,
                Maximum = 1,
                Position = AxisPosition.Left
            });

            BestKnownModel = model;
        }
    }
}
