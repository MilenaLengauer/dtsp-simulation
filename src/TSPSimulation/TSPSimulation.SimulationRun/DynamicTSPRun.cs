using Serilog;
using SimSharp;
using TSPSimulation.Algorithms;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.ProblemDefinition;
using TSPSimulation.ProblemDefinition.Static;
using Process = SimSharp.Process;

namespace TSPSimulation.SimulationRun
{
    public class DynamicTSPRun
    {
        private readonly DynamicTSPConfiguration configuration;
        private readonly DynamicTSP dynamicTSP;

        private readonly IAlgorithm algorithm;
        private readonly DynamicTSPSolution dynamicTSPSolution;

        private event EventHandler<SimulationUpdateEventData>? SimulationUpdateEvent;

        private TSP currentProblem;        
        private int currentStart;
        private IList<int> visistedCities = new List<int>();
        private bool driveEnded = false;

        public DynamicTSPRun(DynamicTSP dynamicTSP, string runName, AlgorithmConfiguration algorithmConfiguration, EventHandler<SimulationUpdateEventData>? simulationUpdateEvent = null)
        {
            this.dynamicTSP = dynamicTSP;
            configuration = dynamicTSP.Configuration;
            this.dynamicTSPSolution = new DynamicTSPSolution(runName, dynamicTSP, algorithmConfiguration);
            currentProblem = dynamicTSP.GetProblem(0);
            algorithm = algorithmConfiguration.CreateAlgorithm(currentProblem);
            SimulationUpdateEvent = simulationUpdateEvent;
        }

        public DynamicTSPSolution ExecuteSimulation()
        {
            Log.Logger.Information("Starting simulation...");
            dynamicTSPSolution.AlgorithmReport.ReportNewPeriod(currentProblem, null);
            dynamicTSPSolution.AlgorithmReport.ReportNewGeneration(algorithm.InitializePopulation());

            var env = new Simulation();

            SimulationUpdateEvent?.Invoke(this, new SimulationUpdateEventData(env.NowD, $"Started at {DateTime.Now:HH:mm:ss}", 0));
            Initialize();
            SimulationUpdateEvent?.Invoke(this, new SimulationUpdateEventData(env.NowD, "calculated initial solution", 0));

            Process driverProcess = env.Process(Driver(env));
            Process solutionProcess = env.Process(SolutionCreator(env));
            env.Process(ProblemSetter(env, driverProcess, solutionProcess));
            env.Run();

            dynamicTSPSolution.AlgorithmReport.FinishReport();

            SimulationUpdateEvent?.Invoke(this, new SimulationUpdateEventData(env.NowD, $"finished: {string.Join("-", dynamicTSPSolution.FinalPath)}", dynamicTSPSolution.FinalPath.Count));

            Log.Logger.Information("Simulation finished.");

            return dynamicTSPSolution;
        }

        private void Initialize()
        {
            while(algorithm.Evaluations < configuration.TimeForInitialSolution)
            {
                GenerationResult? result = algorithm.Run();
                if (result != null)
                {
                    dynamicTSPSolution.AlgorithmReport.ReportNewGeneration(result);
                }
            }
            dynamicTSPSolution.AlgorithmReport.ReportInitializationCompleted();
            Log.Logger.Information("Executed initialization phase of algorithm");
        }

        private IEnumerable<Event> SolutionCreator(Simulation env)
        {
            bool optimum = false;
            while (!driveEnded && !optimum)
            {
                int currentEvaluations = algorithm.Evaluations;
                GenerationResult? result = algorithm.Run();
                if(result == null)
                {
                    optimum = true;
                    Log.Logger.Information("Sovler: optimum already found, stopping solver process");
                } else
                {
                    dynamicTSPSolution.AlgorithmReport.ReportNewGeneration(result); 
                    yield return env.TimeoutD(result.Evaluations -  currentEvaluations);
                }
            }
        }

        private IEnumerable<Event> ProblemSetter(Simulation env, Process driverProcess, Process solutionProcess)
        {
            int i = 1;
            while (i < dynamicTSP.Problems.Count)
            {
                yield return env.TimeoutD(dynamicTSP.Problems[i].time - env.NowD);

                if(driveEnded && AllCitiesRevealed(env.NowD))
                {
                    Log.Logger.Information("ProblemSetter: driver process stopped");
                    break;
                }
                Log.Logger.Information("Setting new problem");

                UpdateCurrentProblem(dynamicTSP.Problems[i].tspUpdate.Problem);
                GenerationResult? result = algorithm.SetNewProblem(currentProblem, Array.Empty<int>(), dynamicTSP.Problems[i].tspUpdate.AddedCities);
                dynamicTSPSolution.AlgorithmReport.ReportNewPeriod(currentProblem, result);

                // when new cities are revealed and the driver process already ended, restart the driver
                if (driverProcess.IsProcessed && (dynamicTSP.Problems[i].tspUpdate.Type == TSPUpdateType.NewCities || dynamicTSP.Problems[i].tspUpdate.Type == TSPUpdateType.BothChanged))
                {
                    Log.Logger.Information("ProblemSetter: restarting driver process, as new cities were revealed");
                    driverProcess = env.Process(Driver(env));
                    solutionProcess = env.Process(SolutionCreator(env));
                } else if(solutionProcess.IsProcessed)
                {
                    Log.Logger.Information("ProblemSetter: restarting solver process");
                    solutionProcess = env.Process(SolutionCreator(env));
                }
                i++;
            }
        }

        private void UpdateCurrentProblem(TSP problem)
        {
            int[] newCities = problem.Cities.Where(c => !visistedCities.Contains(c)).ToArray();
            currentProblem = new TSP(problem.DistanceMatrix, currentStart, problem.End, newCities, problem.Coordinates);
        }

        private IEnumerable<Event> Driver(Simulation env)
        {
            driveEnded = false;
            while (!driveEnded)
            {
                int currentCity = currentProblem.Start;
                Log.Logger.Information("Driver: current city: " + currentCity);

                SimulationUpdateEvent?.Invoke(this, new SimulationUpdateEventData(env.NowD, $"reached city {currentCity}", dynamicTSPSolution.FinalPath.Count));

                int idxCurrentCity = Array.IndexOf(algorithm.CurrentBestSolution.Path, currentCity);
                int nextCity;
                if(idxCurrentCity + 1 >= algorithm.CurrentBestSolution.Path.Length)
                {
                    nextCity = currentProblem.End;
                }else
                {
                    nextCity = algorithm.CurrentBestSolution.Path[idxCurrentCity + 1];
                }
                Log.Logger.Information("Driver: next city: " + nextCity);
                double distance = currentProblem.DistanceMatrix[currentCity, nextCity];

                VisitedCity(currentCity, nextCity, distance);
                int[] removed = { nextCity };
                dynamicTSPSolution.AlgorithmReport.ReportNewGeneration(algorithm.SetNewProblem(currentProblem, removed, Array.Empty<int>()));

                yield return env.TimeoutD(distance * configuration.TimeForDistanceUnit);

                if (nextCity == currentProblem.End && currentProblem.Cities.Length == 0)
                {
                    Log.Logger.Information("Driver: Stopping driver process");
                    driveEnded = true;
                }
            }
        }

        private void VisitedCity(int currentCity, int nextCity, double distance)
        {
            dynamicTSPSolution.AddToFinalPath(nextCity, distance, algorithm.CurrentGeneration + 1);
            dynamicTSPSolution.AlgorithmReport.VisitedCity(new Edge(currentCity, nextCity), (double)distance);
            currentStart = nextCity;
            visistedCities.Add(nextCity);
            int[] cities = Array.FindAll(currentProblem.Cities, c => c != nextCity).ToArray();
            currentProblem = new TSP(currentProblem.DistanceMatrix, nextCity, currentProblem.End, cities, currentProblem.Coordinates);
        }

        private bool AllCitiesRevealed(double time)
        {
            return time >= configuration.RevealCitiesXTimes * configuration.FrequencyRevealCities;
        }

    }
}
