using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ProblemDefinition
{
    public record TSPUpdate(TSPUpdateType Type, int[] AddedCities, TSP Problem)
    {
    }

    public enum TSPUpdateType
    {
        Initial,
        DistanceMatrixChange,
        NewCities,
        BothChanged
    }
}
