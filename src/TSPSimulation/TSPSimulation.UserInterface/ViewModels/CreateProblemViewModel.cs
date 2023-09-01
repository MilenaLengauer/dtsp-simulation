using Docker.DotNet.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.SimulationRun;
using TSPSimulation.UserInterface.Models;
using TSPSimulation.UserInterface.Navigation;
using TSPSimulation.UserInterface.Util;

namespace TSPSimulation.UserInterface.ViewModels
{
    public class CreateProblemViewModel : NotifyPropertyChanged, IBaseViewModel
    {
        public DynamicTSPConfiguration? Configuration { get; set; }

        private IEnumerable<string>? properties;
        public IEnumerable<string>? Properties { 
            get => this.properties; 
            private set => Set(ref this.properties, value);
        }

        private string? problemFilePath;
        public string? ProblemFilePath 
        {
            get => this.problemFilePath;
            set => Set(ref this.problemFilePath, value); 
        }

        public RunModel? RunModel { get; set; }
       
        public ICommand CreateProblemCommand { get; }
        //public ICommand CreateProblemFromFileCommand { get; }
        public ICommand BrowseFileCommand { get; }
        public ICommand ImportRunsCommand { get; }

        private readonly Func<RunModel, SimulationViewModel> newSimulationViewModel;

        public INavigationController NavigationController { get; set; }
        public string Heading { get; set; } = "Create Problem";

        private ThreadSafeRandom? random;

        public CreateProblemViewModel(INavigationController navigationController, Func<RunModel, SimulationViewModel> newSimulationViewModel)
        {
            this.newSimulationViewModel = newSimulationViewModel;
            NavigationController = navigationController;
            Configuration = new DynamicTSPConfiguration("berlin52",7542,20000);
            BrowseFileCommand = new AsyncDelegateCommand(BrowseFile, _ => true);
            CreateProblemCommand = new AsyncDelegateCommand(CreateProblem, _ => Configuration != null);
            //CreateProblemFromFileCommand = new AsyncDelegateCommand(CreateProblemFromFile, _ => ProblemFilePath != null);
            ImportRunsCommand = new AsyncDelegateCommand(ImportRuns, _ => true);
        }

        public Task OnAfterNavigate()
        {
            return Task.CompletedTask;
        }

        public async Task ImportRuns(object _)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "run";
            dialog.DefaultExt = ".json";
            dialog.Filter = "Json files (*.json)|*.json";
            bool? result = dialog.ShowDialog();

            if(result == true)
            {
                using (StreamReader file = File.OpenText(dialog.FileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    serializer.ContractResolver = new JsonPrivateSetterResolver();
                    RunModel? model = serializer.Deserialize(file, typeof(RunModel)) as RunModel;
                    if (model != null)
                    {
                        RunModel = model;
                        RunModel.RepairAfterImport();
                        await NavigationController.Navigate(newSimulationViewModel(RunModel));
                    }
                }
            }
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
                ProblemFilePath = dialog.FileName;
                using (StreamReader r = new StreamReader(ProblemFilePath))
                {
                    string json = r.ReadToEnd();
                    Configuration = JsonConvert.DeserializeObject<DynamicTSPConfiguration>(json);
                    Properties = Configuration?.GetType().GetProperties()
                        .Select(propertyInfo => $"{propertyInfo.Name}: {propertyInfo.GetValue(Configuration, null) ?? "(null)"}");
                }
            }
        }

        public async Task CreateProblem(object _)
        {
            if (Configuration == null) return;
            if(Configuration.RandomState == null)
            {
                random = new ThreadSafeRandom();
            } else
            {
                random = new ThreadSafeRandom(Configuration.RandomState.Value);
            }
            DynamicTSPCreator tspCreator = new DynamicTSPCreator(Configuration, random);
            RunModel = new RunModel(tspCreator.CreateDynamicTSP());

            await NavigationController.Navigate(newSimulationViewModel(RunModel));
        }
    }
}
