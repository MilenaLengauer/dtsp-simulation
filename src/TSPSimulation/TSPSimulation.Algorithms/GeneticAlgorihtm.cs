using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;
using TSPSimulation.Algorithms.Configuration;

namespace TSPSimulation.Algorithms
{
    public class GeneticAlgorihtm : BaseAlgorithm
    {

        public GeneticAlgorihtm(TSP tsp, GeneticAlgorithmConfiguration configuration, ThreadSafeRandom random)
            : base(tsp, configuration, random)
        {
        }
    }
}
