using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ProblemDefinition
{
    public record StaticTSPAtTimestamp(TSP problem, decimal Time, int Generation)
    {
    }
}
