using Docker.DotNet.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels
{
    public class ShowProblemViewModel : NotifyPropertyChanged
    {
        public DynamicTSP DynamicTSP { get; private set; }
        public DynamicTSPSolution? Solution { get; private set; }
        public PlotModel TSPMapModel { get; private set; }

        private long sliderValue = 0;
        public long SliderValue {
            get => this.sliderValue;
            set {
                Set(ref this.sliderValue, value);
                SliderValueUpdated();
            }
        }
        public int SliderMin { get; private set; } = 0;
        public long SliderMax { get; private set; }
        public int SliderStepSize { get; private set; }

        public IEnumerable<string>? Properties { get; }

        public ShowProblemViewModel(DynamicTSP problem, DynamicTSPSolution? solution = null, bool showProperties = true)
        {
            DynamicTSP = problem;
            Solution = solution;

            if(Solution == null)
                SliderMax = DynamicTSP.Configuration.TimeLastCityReveal();
            else
                SliderMax = Solution.GetFinalTime();

            SliderStepSize = 1;

            TSPMapModel = new PlotModel();
            TSPMapModel.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.None,
                Palette = new OxyPalette(OxyColors.Blue, OxyColors.Gray, OxyColors.Red)
            });
            SliderValueUpdated();

            if(showProperties)
            {
                Properties = DynamicTSP.Configuration.GetType().GetProperties()
                .Select(propertyInfo => $"{propertyInfo.Name}: {propertyInfo.GetValue(DynamicTSP.Configuration, null) ?? "(null)"}");
            }
        }

        public void SliderValueUpdated()
        {
            TSPMapModel.Series.Clear();

            UpdateProblemSeries(sliderValue);
            UpdateSolutionSeries(sliderValue);

            TSPMapModel.InvalidatePlot(true);
        }

        public void UpdateProblemSeries(long time)
        {
            var problem = DynamicTSP.GetProblemUpdate(time).Problem;
            ScatterSeries scatterSeries = new ScatterSeries();
            for (int i = 0; i < problem.Coordinates.Length; i++)
            {
                ScatterPoint scatterPoint = new ScatterPoint(
                    problem.Coordinates[i].X,
                    problem.Coordinates[i].Y,
                    3);
                if (i == 0)
                {
                    scatterPoint.Value = 0;
                }
                else if (problem.Cities.Contains(i))
                {
                    scatterPoint.Value = 2;
                }
                else
                {
                    scatterPoint.Value = 1;
                }
                scatterSeries.Points.Add(scatterPoint);
            }
            TSPMapModel.Series.Add(scatterSeries);
        }
        
        public void UpdateSolutionSeries(long time)
        {
            if (Solution == null) return; // no solution layer in the chart

            IEnumerable<int> reachedCities = Solution.GetReachedCities(time);

            LineSeries lineSeries = new LineSeries();
            foreach(int city in reachedCities)
            {
                lineSeries.Points.Add(new DataPoint(DynamicTSP.GetCoordinates(city).X, DynamicTSP.GetCoordinates(city).Y));
            }
            TSPMapModel.Series.Add(lineSeries);
        }
    }
}
