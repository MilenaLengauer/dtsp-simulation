using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Configuration
{
    public class RestartGeneticAlgorithmConfiguration : AlgorithmConfiguration
    {
        public override IAlgorithm CreateAlgorithm(TSP tsp)
        {
            return new RestartGeneticAlgorithm(tsp, this, RandomState.HasValue ? new ThreadSafeRandom(RandomState.Value) : new ThreadSafeRandom());
        }

        public override string GetAlgorithmName()
        {
            return "Restart Genetic Algorithm";
        }
    }
}
