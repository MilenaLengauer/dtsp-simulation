using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Selectors
{
    public interface ISelector
    {
        int Select(ThreadSafeRandom random, TSPSolution[] population);
    }
}
