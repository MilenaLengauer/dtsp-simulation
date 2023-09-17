using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.ExternalSolvers;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Reporting
{
    public class PeriodReport
    {
        private const double TOLERANCE = 0.001;

        public string Id { get; private set; }
        public int PeriodNumber { get; private set; }
        public TSP Problem { get; private set; }

        public int StartGeneration { get; private set; }
        public int EndGeneration { get; internal set; }
        public IList<GenerationReport> Generations { get; private set; }

        public double AlreadyDriven { get; private set; } = 0;
        public IList<Edge> AlreadyDrivenEdges { get; private set; } = new List<Edge>();

        [JsonIgnore]
        public AlgorithmReport AlgorithmReport { get; set; }


        // lazy load metrics
        private TSPSolution finalSolution;
        [JsonIgnore]
        public TSPSolution FinalSolution
        {
            get => finalSolution ??= GetFinalSolution();
        }

        private IEnumerable<(int evaluations, double fitness)> bestFitnesses;
        [JsonIgnore]
        public IEnumerable<(int evaluations, double fitness)> BestFitnesses
        {
            get => bestFitnesses ??= GetBestFitnesses();
        }

        private IEnumerable<(int evaluations, double error)> errorsInPercent;
        [JsonIgnore]
        public IEnumerable<(int evaluations, double error)> ErrorsInPercent
        {
            get => errorsInPercent ??= GetErrorsInPercent();
        }

        private IEnumerable<(int evaluations, double accuracy)> accuracies;
        [JsonIgnore]
        public IEnumerable<(int evaluations, double accuracy)> Accuracies
        {
            get => accuracies ??= GetAccuracies();
        }

        private double? recoveryRate;
        [JsonIgnore]
        public double RecoveryRate
        {
            get => recoveryRate ??= GetRecoveryRate();
        }

        private double? absoluteRecoveryRate;
        [JsonIgnore]
        public double AbsoluteRecoveryRate
        {
            get => absoluteRecoveryRate ??= GetAbsoluteRecoveryRate();
        }

        private double? stability;
        [JsonIgnore]
        public double? Stability
        {
            get => stability ??= GetStability();
        }

        public PeriodReport(int periodNumber, TSP problem, string name, AlgorithmReport algorithmReport)
        {
            PeriodNumber = periodNumber;
            Problem = problem;
            Id = $"{name?.Replace(' ', '_')}_{PeriodNumber}";
            Generations = new List<GenerationReport>();
            AlgorithmReport = algorithmReport;
        }

        internal void VisitedCity(Edge edge, double distance)
        {
            AlreadyDriven += distance;
            AlreadyDrivenEdges.Add(edge);
        }

        internal void CalculateOptimalSolution()
        {
            int[] optimalPath;
            if (Problem.ProblemSize > 1)
            {
                ISolver concordeCaller = new ConcordeDockerSolver(Id, Problem);
                optimalPath = concordeCaller.Solve();
            }
            else
            {
                List<int> path = new List<int>();
                path.AddRange(Problem.Cities);
                optimalPath = path.ToArray();
            }
            Problem.OptimalSolution = new TSPSolution(optimalPath, Problem);
            Problem.OptimalSolution.Evaluate();
        }

        public void CalculateWorstSolution()
        {
            int[] worstPath;
            if(Problem.ProblemSize > 1)
            {
                worstPath = new int[Problem.Cities.Length];
                List<int> cities = new List<int>();
                cities.AddRange(Problem.Cities);
                int currentCity = Problem.Start;
                for(int i = 0; i < worstPath.Length; i++)
                {
                    //find most distant city
                    double maxDist = 0;
                    int maxDistCity = -1;
                    for(int j = 0; j < cities.Count; j++)
                    {
                        double dist = Problem.DistanceMatrix[currentCity, cities[j]];
                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            maxDistCity = cities[j];
                        }
                    }
                    cities.Remove(maxDistCity);
                    worstPath[i] = maxDistCity;
                    currentCity = worstPath[i];
                }

            } else
            {
                List<int> path = new List<int>();
                path.AddRange(Problem.Cities);
                worstPath = path.ToArray();
            }
            Problem.WorstSolution = new TSPSolution(worstPath, Problem);
            Problem.WorstSolution.Evaluate();
        }

        internal void ReportNewGeneration(GenerationResult generation, bool performDiversityCalculations, bool performEdgeFrequencyCalculations)
        {
            if(Generations.Count == 0)
            {
                StartGeneration = generation.Generation;
            }
            Generations.Add(
                new GenerationReport(generation, this, performDiversityCalculations, performEdgeFrequencyCalculations)
                );
        }

        public GenerationReport? GetGenerationReport(int generation)
        {
            return Generations.Where(g => g.Generation == generation).FirstOrDefault();
        }

        private TSPSolution GetFinalSolution()
        {
            List<int> path = new List<int>();

            foreach (var edge in AlreadyDrivenEdges)
            {
                path.Add(edge.City2);
            }

            if (path.Count > 0 && path[^1] == Problem.End)
            {
                path.RemoveAt(path.Count - 1);
            }
            else
            {
                GenerationReport generation = Generations.Last();
                path.AddRange(generation.BestSolution.Path);
            }
            var solution = new TSPSolution(path.ToArray(), Problem);
            solution.Evaluate();
            return solution;
        }

        private IEnumerable<(int evaluations, double fitness)> GetBestFitnesses()
        {
            IList<(int, double)> bestFitnesses = new List<(int, double)> ();
            foreach(var generation in Generations)
            {
                bestFitnesses.Add((generation.Evaluations, generation.BestFitnessInPeriod));
            }
            return bestFitnesses;
        }

        public IEnumerable<(int evaluations, double error)> GetErrorsInPercent()
        {
            double min = Problem.OptimalSolution.Fitness;
            IList<(int, double)> errors = new List<(int, double)>();
            foreach (var generation in Generations)
            {
                if(min < TOLERANCE)
                {
                    errors.Add((generation.Evaluations, 0));
                } else
                {
                    errors.Add((generation.Evaluations, (generation.BestFitnessInPeriod - min) / min));
                }
                
            }
            return errors;
        }

        public IEnumerable<(int, double)> GetAccuracies()
        {
            double min = Problem.OptimalSolution.Fitness;
            double max = Problem.WorstSolution.Fitness;
            double diff = max - min;
            IList<(int, double)> accuracies = new List<(int, double)>();
            foreach (var generation in Generations)
            {
                if(diff < TOLERANCE)
                {
                    accuracies.Add((generation.Evaluations, 0));
                } else
                {
                    accuracies.Add((generation.Evaluations, (max - generation.BestFitnessInPeriod) / diff));
                }
            }
            return accuracies;

        }

        private double GetRecoveryRate()
        {
            double fitnessStart = Generations.First().BestFitnessInPeriod;

            double bestFitnessSoFar = double.MaxValue;
            double sumDifferences = 0;

            foreach (var generation in Generations)
            {
                if(generation.BestFitnessInPeriod < bestFitnessSoFar)
                {
                    bestFitnessSoFar = generation.BestFitnessInPeriod;
                }
                sumDifferences += (fitnessStart - bestFitnessSoFar);
            }
            if((fitnessStart - bestFitnessSoFar) < TOLERANCE)
            {
                return 1;
            }
            return sumDifferences / (Generations.Count * (fitnessStart - bestFitnessSoFar));
        }

        private double GetAbsoluteRecoveryRate()
        {
            double fitnessStart = Generations.First().BestFitnessInPeriod;

            double bestFitnessSoFar = double.MaxValue;
            double sumDifferences = 0;

            foreach (var generation in Generations)
            {
                if (generation.BestFitnessInPeriod < bestFitnessSoFar)
                {
                    bestFitnessSoFar = generation.BestFitnessInPeriod;
                }
                sumDifferences += (fitnessStart - bestFitnessSoFar);
            }
            if ((fitnessStart - Problem.OptimalSolution.Fitness) < TOLERANCE)
            {
                return 1;
            }
            return sumDifferences / (Generations.Count * (fitnessStart - Problem.OptimalSolution.Fitness));
        }

        private double? GetStability()
        {
            if (PeriodNumber == 0) return null;
            PeriodReport previousPeriod = AlgorithmReport.Periods[PeriodNumber - 1];
            var previousErrors = previousPeriod.ErrorsInPercent;
            if (previousErrors.Count() == 0 || ErrorsInPercent.Count() == 0) return null;

            double stability = ErrorsInPercent.First().error - previousErrors.Last().error;
            return stability < 0 ? 0 : stability;
        }

    }
}
