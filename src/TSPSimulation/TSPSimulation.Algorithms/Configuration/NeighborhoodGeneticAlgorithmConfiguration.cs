using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Configuration
{
    public class NeighborhoodGeneticAlgorithmConfiguration : AlgorithmConfiguration
    {
        public int NeighborhoodSize { get; set; } = 10;
        public int? NeighborhoodSizeMin { get; set; } = null;
        public int? NeighborhoodSizeMax { get; set; } = null;
        public double DiversityEnhancementSameAsLead { get; set; } = 1;
        public double DiversityEnhancementReplace { get; set; } = 0.5;
        public bool EnableLocalSearch { get; set; } = true;
        public bool MutationDiversityEnhancement { get; set; } = false;

        public override IAlgorithm CreateAlgorithm(TSP tsp)
        {
            return new NeighborhoodGeneticAlgorithm(tsp, this, RandomState.HasValue ? new ThreadSafeRandom(RandomState.Value) : new ThreadSafeRandom());
        }

        public override string GetAlgorithmName()
        {
            return "Neighborhood Genetic Algorithm";
        }
    }
}
