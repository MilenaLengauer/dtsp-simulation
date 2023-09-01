using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SimSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TSPSimulation.Reporting;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels.CompareRunsViewModels
{
    public class RecoveryRateViewModel : NotifyPropertyChanged
    {
        private PlotModel? chart;
        public PlotModel? Chart
        {
            get => chart;
            private set => Set(ref chart, value);
        }

        private IEnumerable<AlgorithmReport> reports;

        public ICommand ReloadCommand { get; }

        public RecoveryRateViewModel(RunModel runModel)
        {
            reports = runModel.Solutions.Select(s => s.AlgorithmReport);
            ReloadCommand = new AsyncDelegateCommand(DrawRecoveryRates, _ => true);
        }

        public async Task DrawRecoveryRates(object _)
        {
            var model = new PlotModel();

            var groupedReports = reports.GroupBy(r => r.Configuration.Group);

            //model.Axes.Add(new LinearColorAxis
            //{
            //    Position = AxisPosition.Bottom,
            //    Palette = OxyPalettes.Rainbow(groupedReports.Count())
            //});

            Func<double, double> function = (x) => x;
            model.Series.Add(new FunctionSeries(function, 0, 1, 0.1));
            int i = 0;
            foreach (var group in groupedReports)
            {
                var scatterSeries = new ScatterSeries { Title = $"Group {group.Key}" };
                foreach (var report in group)
                {
                    scatterSeries.Points.Add(new ScatterPoint(report.RecoveryRate, report.AbsoluteRecoveryRate, 5));
                }
                model.Series.Add(scatterSeries);
                i++;
            }

            
            
            model.Legends.Add(new Legend { LegendPosition = LegendPosition.TopLeft, IsLegendVisible = true });

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            Chart = model;
        }
    }
}
