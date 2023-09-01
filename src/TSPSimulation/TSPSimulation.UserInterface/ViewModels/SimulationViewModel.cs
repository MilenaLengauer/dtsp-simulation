using CsvHelper;
using Microsoft.Win32;
using Newtonsoft.Json;
using SimSharp;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Navigation;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels
{
    public class SimulationViewModel : NotifyPropertyChanged, IBaseViewModel
    {
        public INavigationController NavigationController { get; set; }
        public string Heading { get; set; } = "Simulation";

        public ShowProblemViewModel ShowProblemViewModel { get; set; }
        public AlgorithmViewModel AlgorithmViewModel { get; set; }
        public RunsViewModel RunsViewModel { get; set; }
        public CompareRunsViewModel CompareRunsViewModel { get; set; }
        public ICommand ReconfigureProblemCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ExportToCsvCommand { get; }

        private CreateProblemViewModel problemView;

        private RunModel runModel;

        public SimulationViewModel(RunModel runModel, INavigationController navigationController, CreateProblemViewModel problemView)
        {
            this.runModel = runModel;
            NavigationController = navigationController;
            ShowProblemViewModel = new ShowProblemViewModel(runModel.DynamicTSP);
            RunsViewModel = new RunsViewModel(runModel);
            AlgorithmViewModel = new AlgorithmViewModel(runModel);
            CompareRunsViewModel = new CompareRunsViewModel(runModel);
            this.problemView = problemView;

            ReconfigureProblemCommand = new AsyncDelegateCommand(ResetProblem, _ => true);
            ExportCommand = new AsyncDelegateCommand(Export, _ => true);
            ExportToCsvCommand = new AsyncDelegateCommand(ExportToCsv, _ => true);
        }

        public async Task Export(object _)
        {
            //string json = JsonConvert.SerializeObject(selectedSolution);

            SaveFileDialog saveFileDialog1 = new()
            {
                Filter = "Json files (*.json)|*.json",
                FileName = "run",
                DefaultExt = ".json",
                Title = "Save Run"
            };
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using TextWriter writer = File.CreateText(saveFileDialog1.FileName);
                var serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.Serialize(writer, runModel);
                //File.WriteAllText(saveFileDialog1.FileName, json);                
            }

        }

        public async Task ExportToCsv(object _)
        {
            SaveFileDialog saveFileDialog1 = new()
            {
                Filter = "CSV file (*.csv)|*.csv",
                FileName = "results",
                DefaultExt = ".csv",
                Title = "Export Results"
            };
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using var writer = new StreamWriter(saveFileDialog1.FileName);
                using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
                CsvExportResult export = new CsvExportResult(runModel);
                csv.WriteRecords(export.AlgorithmResults);               
            }

        }

        public Task OnAfterNavigate()
        {
            return Task.CompletedTask;
        }

        public async Task ResetProblem(object _)
        {
            await NavigationController.Navigate(problemView);
        }
    }
}
