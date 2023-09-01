using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ExternalSolvers
{
    public interface ISolver
    {
        int[] Solve();
        Task<int[]> SolveAsync();
    }
}
