using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.ALPS
{
    public class AgeHistogramViewModel : NotifyPropertyChanged
    {
        private PlotModel? ageModel;
        public PlotModel? AgeModel
        {
            get => this.ageModel;
            private set => Set(ref this.ageModel, value);
        }

        private AlgorithmReport algorithmReport;

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

        public AgeHistogramViewModel(DynamicTSPSolution solution)
        {
            this.algorithmReport = solution.AlgorithmReport;

            SliderMin = 0;
            SliderMax = algorithmReport.GetGenerationReports().Last().Generation;

            AgeModel = new PlotModel();
            BuildChart(0);
        }

        private void BuildChart(int generation)
        {
            var model = new PlotModel();
            var generationReport = algorithmReport.GetGenerationReport(generation);
            if (generationReport == null || generationReport.AgeHistogram == null) return;

            BarSeries ageSeries = new BarSeries();
            CategoryAxis ageBinAxis = new CategoryAxis { Position = AxisPosition.Left };
            foreach (var age in generationReport.AgeHistogram)
            {
                ageSeries.Items.Add(new BarItem(age.Value));
                ageBinAxis.Labels.Add($"{age.Key}");
            }
            model.Series.Add(ageSeries);
            model.Axes.Add(ageBinAxis);
            AgeModel = model;
        }
        
    }
}
