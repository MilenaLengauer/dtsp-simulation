using TSPSimulation.Algorithms.Crossovers;
using TSPSimulation.Algorithms.Selectors;
using TSPSimulation.ProblemDefinition.Static;
using TSPSimulation.Algorithms.Mutators;
using System;
using TSPSimulation.Commons;

namespace TSPSimulation.Algorithms.Configuration
{
    public class GeneticAlgorithmConfiguration : AlgorithmConfiguration
    {
        public override IAlgorithm CreateAlgorithm(TSP tsp)
        {
            return new GeneticAlgorihtm(tsp, this, RandomState.HasValue ? new ThreadSafeRandom(RandomState.Value) : new ThreadSafeRandom());
        }

        public override string GetAlgorithmName()
        {
            return "Genetic Algorithm";
        }
    }
}
