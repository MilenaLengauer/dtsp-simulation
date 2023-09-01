using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.Generic;
using System.Linq;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.UserInterface.Util;
using TSPSimulation.Commons;
using TSPSimulation.Reporting;
using System.Windows.Input;
using System.Threading.Tasks;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.ALPS
{
    public class LayersDiversityHeatMapViewModel : NotifyPropertyChanged
    {
        public PlotModel ColorsModel { get; private set; }

        private PlotModel? heatMapModel;
        public PlotModel? HeatMapModel {
            get => this.heatMapModel;
            private set => Set(ref this.heatMapModel, value);
        }

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

        public ICommand ExportCommand { get; }

        private AlgorithmReport algorithmReport;

        private Dictionary<int, PlotModel> charts = new Dictionary<int, PlotModel>();
        private ThreadSafeRandom random;

        private ChartPrinter chartPrinter = new ChartPrinter();

        public LayersDiversityHeatMapViewModel(DynamicTSPSolution solution)
        {
            this.algorithmReport = solution.AlgorithmReport;

            ExportCommand = new AsyncDelegateCommand(PrintToPdf, _ => true);

            SliderMin = 0;
            SliderMax = algorithmReport.GenerationReportsWithDiversity().LastOrDefault()?.Generation ?? 0;

            random = new ThreadSafeRandom();

            ColorsModel = CreateColorModel(100);
            BuildChart(0);
        }

        private void BuildChart(int generation)
        {
            if(charts.ContainsKey(generation))
            {
                HeatMapModel = charts[generation];
                return;
            }
            GenerationReport? generationReport = algorithmReport.GetGenerationReport(generation);
            if(generationReport == null || generationReport.LayerDiversityMap == null)
            {
                return;
            }
            var model = new PlotModel();
            double[,] diversityMap = generationReport.LayerDiversityMap;

            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(100), Maximum = 1, Minimum = 0
            });

            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = diversityMap.GetLength(0) - 1,
                Y0 = 0,
                Y1 = diversityMap.GetLength(0) - 1,
                Interpolate = false,
                RenderMethod = HeatMapRenderMethod.Rectangles,
                Data = diversityMap
            };

            model.Series.Add(heatMapSeries);
            model.DefaultFontSize = 14;
            HeatMapModel = model;
            charts[generation] = model;
            
        }

        private PlotModel CreateColorModel(int numberOfColors)
        {
            var model = new PlotModel();

            double[,] colorData = new double[numberOfColors, numberOfColors];
            for (int i = 0; i < numberOfColors; i++)
            {
                for (int j = 0; j < numberOfColors; j++)
                {
                    colorData[i, j] = j;
                }

            }
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(numberOfColors)
            });
            model.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                IsAxisVisible = false
            });
            model.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            });
            var colorSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = numberOfColors - 1,
                Y0 = 0,
                Y1 = numberOfColors - 1,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = colorData
            };
            model.Series.Add(colorSeries);
            return model;
        }

        public async Task PrintToPdf(object _)
        {
            chartPrinter.PrintChart(HeatMapModel, 300, 300);
            chartPrinter.PrintChart(ColorsModel, 50, 300);
        }
    }
}
