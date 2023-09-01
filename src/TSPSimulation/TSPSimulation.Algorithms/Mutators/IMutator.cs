using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Mutators
{
    public interface IMutator
    {
        void Mutate(ThreadSafeRandom random, TSPSolution tspSolution);
    }
}
