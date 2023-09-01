using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Reporting
{
    public class GenerationReport
    {
        private const double TOLERANCE = 0.001;

        public int Generation { get; private set; }
        public int Evaluations { get; private set; } // evaluations so far

        [JsonIgnore]
        public PeriodReport? Period { get; set; }

        public TSPSolution BestSolution { get; private set; }
        public double BestFitness { get; private set; }
        public double BestFitnessInPeriod { get; private set; }
        public double AverageFitness { get; private set; }
        public double AverageFitnessInPeriod { get; private set; }
        public double WorstFitness { get; private set; }
        public double WorstFitnessInPeriod { get; private set; }

        public double? PopulationDiversity { get; private set; }
        public double[,]? PopulationDiversityMap { get; private set; }

        public int? UniqueEdgesCount { get; private set; }
        public int? FixedEdgesCount { get; private set; }
        public int? FixedEdgesOfBestKnownCount { get; private set; }
        public int? LostEdgesOfBestKnownCount { get; private set; }
        public double? AverageContainedBestKnownEdges { get; private set; }

        private TSP? problem;

        private IDictionary<Edge, EdgeFrequency>? edgeFrequencies;
        private IDictionary<string, EdgeFrequency>? exportEdgeFrequencies;
        public IDictionary<string, EdgeFrequency>? ExportEdgeFrequencies
        {
            get
            {
                if (exportEdgeFrequencies == null && edgeFrequencies != null)
                {
                    exportEdgeFrequencies = edgeFrequencies.ToDictionary(pair => $"{pair.Key.City1}-{pair.Key.City2}", pair => pair.Value);
                }
                return exportEdgeFrequencies;
            }
            private set { exportEdgeFrequencies = value; }
        }

        //NGA specific
        public IList<double> NeighborhoodBests { get; private set; } = new List<double>();
        public IList<double> NeighborhoodAverages { get; private set; } = new List<double>();
        public IList<double> NeighborhoodWorsts { get; private set; } = new List<double>();
        public IList<double> NeighborhoodDiversities { get; private set; } = new List<double>();
        public double[,]? NeighborhoodDiversityMap { get; private set; }

        //ALPS specific
        public IList<double> AgeLayerBests { get; private set; } = new List<double>();
        public IList<double> AgeLayerAverages { get; private set; } = new List<double>();
        public IList<double> AgeLayerWorsts { get; private set; } = new List<double>();
        public int? OldestAge { get; private set; }
        public int? YoungestAge { get; private set; }
        public double? AverageAge { get; private set; }
        public SortedDictionary<int, int>? AgeHistogram;
        public IList<double> LayerDiversities { get; private set; } = new List<double>();
        public double[,]? LayerDiversityMap { get; private set; }

        public GenerationReport() { }

        public GenerationReport(GenerationResult generationResult, PeriodReport period, bool performDiversityCalculations, bool performEdgeFrequencyCalculations)
        {
            problem = generationResult.Problem;
            Generation = generationResult.Generation;
            Evaluations = generationResult.Evaluations;
            Period = period;

            BestSolution = generationResult.Population[0];
            BestFitness = BestSolution.Fitness;
            BestFitnessInPeriod = BestFitness + period.AlreadyDriven;
            AverageFitness = generationResult.Population.Average(s => s.Fitness);
            AverageFitnessInPeriod = AverageFitness + period.AlreadyDriven;
            WorstFitness = generationResult.Population[^1].Fitness;
            WorstFitnessInPeriod = WorstFitness + period.AlreadyDriven;

            if (performDiversityCalculations)
            {
                (PopulationDiversity, PopulationDiversityMap) = CalculateEdgeDiversity(generationResult.Population);
            }
            if (performEdgeFrequencyCalculations)
            {
                AnalyzeEdgeFrequencies(generationResult.Population);
            }
            if(generationResult is NgaGenerationResult ngaResult)
            {
                AnalyzeNeighborhoods(ngaResult.Neighborhoods, period.AlreadyDriven, performDiversityCalculations);
            }
            if(generationResult is AlpsGenerationResult alpsResult)
            {
                AnalyzeAgeLayers(alpsResult.AgeLayers, period.AlreadyDriven, performDiversityCalculations);
            }
        }

        public double? EdgeFrequency(Edge edge)
        {
            if (edgeFrequencies == null) return null;
            if (!edgeFrequencies.ContainsKey(edge)) return 0;
            return edgeFrequencies[edge].Frequency;
        }

        private (double, double[,]) CalculateEdgeDiversity(TSPSolution[] population)
        {
            double[,] diversityMap = new double[population.Length, population.Length];
            double sumSolutionDistances = 0;
            for (int i = 0; i < population.Length; i++)
            {
                for (int j = 0; j < population.Length; j++)
                {
                    if (i != j)
                    {
                        double solutionDistance = SolutionDistance(population[i], population[j]);
                        sumSolutionDistances += solutionDistance;
                        diversityMap[i, j] = solutionDistance;
                    }
                    else
                    {
                        diversityMap[i, j] = 0;
                    }
                }
            }
            double diversity = sumSolutionDistances / (population.Length * (population.Length - 1));
            return (diversity, diversityMap);
        }

        private double SolutionDistance(TSPSolution solution1, TSPSolution solution2)
        {
            return solution1.Distance(solution2) / (double)(solution1.Problem.ProblemSize + 1);
        }

        private void AnalyzeEdgeFrequencies(TSPSolution[] population)
        {
            edgeFrequencies = new Dictionary<Edge, EdgeFrequency>();
            foreach (var solution in population)
            {
                foreach (var edge in solution.EdgesHashSet())
                {
                    if (!edgeFrequencies.ContainsKey(edge))
                    {
                        edgeFrequencies[edge] = new EdgeFrequency(edge, Period.Problem.OptimalSolution.EdgesHashSet().Contains(edge), population.Length);
                    }
                    edgeFrequencies[edge].ReportOccurence(solution.Fitness);
                }
            }
            int alreadyDrivenEdgesOfBestKnown = 0;
            foreach(var edge in Period.AlreadyDrivenEdges)
            {
                if(Period.Problem.OptimalSolution.EdgesHashSet().Contains(edge))
                {
                    alreadyDrivenEdgesOfBestKnown++;
                }
            }

            var edgeFrequenciesOfBestKnown = edgeFrequencies.Values.Where(ef => ef.EdgeOfBestKnownSolution).ToList();
            UniqueEdgesCount = edgeFrequencies.Count + Period.AlreadyDrivenEdges.Count;
            FixedEdgesCount = edgeFrequencies.Values.Count(ef => Math.Abs(ef.Frequency - 1) < TOLERANCE) + Period.AlreadyDrivenEdges.Count;
            FixedEdgesOfBestKnownCount = edgeFrequenciesOfBestKnown.Count(ef => Math.Abs(ef.Frequency - 1) < TOLERANCE) + alreadyDrivenEdgesOfBestKnown;
            LostEdgesOfBestKnownCount = Period.Problem.OptimalSolution.EdgesHashSet().Count(edge => !edgeFrequencies.ContainsKey(edge) && !Period.AlreadyDrivenEdges.Contains(edge));
            AverageContainedBestKnownEdges = edgeFrequenciesOfBestKnown.Select(ef => ef.Count).Sum() / (double)population.Length;
        }

        private void AnalyzeNeighborhoods(TSPSolution[][] neighborhoods, double alreadyDriven, bool performDiversityCalculations)
        {
            IList<TSPSolution> neighborhoodBests = new List<TSPSolution>();
            foreach(TSPSolution[] neighborhood in neighborhoods)
            {
                neighborhoodBests.Add(neighborhood[0]);
                NeighborhoodBests.Add(neighborhood[0].Fitness + alreadyDriven);
                NeighborhoodAverages.Add(neighborhood.Average(s => s.Fitness) + alreadyDriven);
                NeighborhoodWorsts.Add(neighborhood[^1].Fitness + alreadyDriven);

                if(performDiversityCalculations)
                {
                    // diversity within each neighborhood
                    NeighborhoodDiversities.Add(CalculateEdgeDiversity(neighborhood).Item1);
                }
            }
            if(performDiversityCalculations)
            {
                // diversity between best best solution of each neighborhood
                NeighborhoodDiversityMap = CalculateEdgeDiversity(neighborhoodBests.ToArray()).Item2;
            }
        }

        private void AnalyzeAgeLayers(List<List<AlpsSolution>> ageLayers, double alreadyDriven, bool performDiversityCalculations)
        {
            if (ageLayers.Count == 0) return;
            YoungestAge = ageLayers.First().Min(s => s.Age);
            OldestAge = ageLayers.Last().Max(s => s.Age);

            var allSolutions = ageLayers.SelectMany(layer => layer).ToList();

            AverageAge = allSolutions.Average(s => s.Age);

            int numberOfBins = 10;
            if(YoungestAge.HasValue && OldestAge.HasValue)
            {
                // divide into bins
                int binSize = (OldestAge.Value / numberOfBins) + 1;
                AgeHistogram = new SortedDictionary<int, int>();
                for(int i = 0; i < numberOfBins; i++)
                {
                    int binStart = i * binSize;
                    int binEnd = (i + 1) * binSize - 1;
                    AgeHistogram.Add(binEnd, allSolutions.Where(s => s.Age >= binStart && s.Age <= binEnd).Count());
                }
            }

            foreach(var layer in ageLayers)
            {
                if(layer.Count > 0)
                {
                    AgeLayerBests.Add(layer[0].Solution.Fitness + alreadyDriven);
                    AgeLayerAverages.Add(layer.Average(s => s.Solution.Fitness) + alreadyDriven);
                    AgeLayerWorsts.Add(layer[^1].Solution.Fitness + alreadyDriven);
                } else
                {
                    Console.WriteLine();
                }
                
            }

            if(performDiversityCalculations)
            {
                IList<TSPSolution> layerBests = new List<TSPSolution>();
                foreach (var layer in ageLayers)
                {
                    LayerDiversities.Add(CalculateEdgeDiversity(layer.Select(s => s.Solution).ToArray()).Item1);
                    if(layer.Count > 0)
                    {
                        layerBests.Add(layer.MinBy(s => s.Solution.Fitness).Solution);
                    }
                }
                // diversity between best best solution of each layer
                LayerDiversityMap = CalculateEdgeDiversity(layerBests.ToArray()).Item2;
            }           
        }
    }
}
