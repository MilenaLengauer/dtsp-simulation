using TSPSimulation.Algorithms.Results;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms
{
    public interface IAlgorithm
    {
        TSPSolution CurrentBestSolution { get; }
        int CurrentGeneration { get; }
        int Evaluations { get; }
        GenerationResult InitializePopulation();
        GenerationResult? Run();
        GenerationResult SetNewProblem(TSP tsp, int[] removedVertices, int[] addedVertices);

    }
}
