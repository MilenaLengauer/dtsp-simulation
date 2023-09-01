using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TSPSimulation.SimulationRun;
using TSPSimulation.UserInterface.Util;
using TSPSimulation.ProblemDefinition;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.UserInterface.Models;
using System.Collections.Generic;
using OperationsResearch;
using Serilog;

namespace TSPSimulation.UserInterface.ViewModels
{
    public class AlgorithmViewModel : NotifyPropertyChanged
    {
        private string? algorithmsFilePath;
        public string? AlgorithmsFilePath
        {
            get => this.algorithmsFilePath;
            set => Set(ref this.algorithmsFilePath, value);
        }

        private string simulationStatusText = "";
        public string SimulationStatusText { 
            get => this.simulationStatusText; 
            set => Set(ref this.simulationStatusText, value); 
        }

        public ICommand BrowseFileCommand { get; }
        public ICommand StartSimulationCommand { get; }

        private bool startEnabled = true;
        public bool StartEnabled {
            get => this.startEnabled;
            set => Set(ref this.startEnabled, value);
        }

        private RunModel runModel;

        public AlgorithmViewModel(RunModel runModel)
        {
            BrowseFileCommand = new AsyncDelegateCommand(BrowseFile, _ => true);
            StartSimulationCommand = new AsyncDelegateCommand(StartSimulation, _ => true);
            this.runModel = runModel;
        }

        public async Task BrowseFile(object _)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "problem"; // Default file name
            dialog.DefaultExt = ".json"; // Default file extension
            dialog.Filter = "Json files (*.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                AlgorithmsFilePath = dialog.FileName;
            }
        }

        public async Task StartSimulation(object _)
        {
            AlgorithmConfiguration[]? algorithmConfigs;
            using (StreamReader r = new StreamReader(AlgorithmsFilePath))
            {
                string json = r.ReadToEnd();
                algorithmConfigs = JsonConvert.DeserializeObject<AlgorithmConfiguration[]>(json, 
                    new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.Auto});
            }

            if(algorithmConfigs != null)
            {
                StartEnabled = false;
                var tasks = new List<Task<DynamicTSPSolution>>();
                for(int i = 0; i < algorithmConfigs.Length; i++)
                {
                    string name = algorithmConfigs[i].Name ?? $"Run {runModel.Solutions.Count}";
                    var simulation = new DynamicTSPRun(runModel.DynamicTSP, name, algorithmConfigs[i],
                        (object? sender, SimulationUpdateEventData eventData) => {
                            SimulationStatusText += $"{Math.Floor(eventData.SimulationTime)} - served cities: {eventData.VisistedCities}, {eventData.Text}\n";
                        }
                    );
                    var solution = await Task.Run(() =>
                    {
                        try
                        {
                            return simulation.ExecuteSimulation();
                        }
                        catch(Exception e)
                        {
                            Log.Logger.Error(e, "Error executing simulation");
                            SimulationStatusText += $"Error executing algorithm, it will be skipped. Check log file for details.\n";
                            return null;
                        }
                    });
                    if(solution != null)
                    {
                        runModel.Solutions.Add(solution);
                    }
                    
                }
                StartEnabled = true;
            }
        }

    }
}
