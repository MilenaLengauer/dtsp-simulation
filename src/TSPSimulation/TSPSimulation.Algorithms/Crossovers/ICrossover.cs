using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Crossovers
{
    public interface ICrossover
    {
        TSPSolution Crossover(ThreadSafeRandom random, TSPSolution s1, TSPSolution s2);
    }
}
