using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.Generic;
using System.Linq;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.UserInterface.Util;
using TSPSimulation.Commons;
using TSPSimulation.Reporting;

namespace TSPSimulation.UserInterface.ViewModels.AnalyzeRunsViewModels.NGA
{
    public class NeighborhoodsDiversityHeatMapViewModel : NotifyPropertyChanged
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

        private AlgorithmReport algorithmReport;

        private Dictionary<int, PlotModel> charts = new Dictionary<int, PlotModel>();
        private ThreadSafeRandom random;

        public NeighborhoodsDiversityHeatMapViewModel(DynamicTSPSolution solution)
        {
            this.algorithmReport = solution.AlgorithmReport;

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
            if(generationReport == null || generationReport.NeighborhoodDiversityMap == null)
            {
                return;
            }
            var model = new PlotModel();
            double[,] diversityMap = generationReport.NeighborhoodDiversityMap;

            // it is not possible to show a heat map larger than 100x100, therefore 100 random solutions are selected (in case there are more than 100)
            if(diversityMap.GetLength(0) > 100)
            {
                IList<int> indices = Enumerable.Range(0, diversityMap.GetLength(0)).ToList<int>();
                IList<int> usedIndicies = indices.RandomChoice(100, random);
                double[,] diversityMapRandomPart = new double[100, 100];
                for(int i=0; i< diversityMapRandomPart.GetLength(0); i++)
                {
                    for(int j=0; j<diversityMapRandomPart.GetLength(1); j++)
                    {
                        diversityMapRandomPart[i, j] = diversityMap[usedIndicies[i],usedIndicies[j]];
                    }
                }
                diversityMap = diversityMapRandomPart;
            }


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
    }
}
