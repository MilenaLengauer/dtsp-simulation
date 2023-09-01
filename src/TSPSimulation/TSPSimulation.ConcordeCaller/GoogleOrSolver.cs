using Google.OrTools.ConstraintSolver;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ExternalSolvers
{
    public class GoogleOrSolver : ISolver
    {
        public TSP Problem { get; }

        public GoogleOrSolver(TSP problem)
        {
            Problem = problem;
        }

        public int[] Solve()
        {
            var data = new GoogleOrTspModel(Problem);
            var manager = new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);
            var routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                // Convert from routing variable Index to
                // distance matrix NodeIndex.
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return data.DistanceMatrix[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            Assignment solution = routing.SolveWithParameters(searchParameters);
            return CreateTSPSolution(routing, manager, solution);
        }

        public Task<int[]> SolveAsync()
        {
            var solution = Solve();
            return Task.FromResult(solution);
        }

        private int[] CreateTSPSolution(RoutingModel routing, RoutingIndexManager manager, Assignment solution)
        {
            var path = new int[Problem.ProblemSize];
            int i = 0;
            var index = routing.Start(0);
            index = solution.Value(routing.NextVar(index)); // skip depot
            while (!routing.IsEnd(index) && i < path.Length)
            {
                path[i] = manager.IndexToNode((int)index);
                i++;
                index = solution.Value(routing.NextVar(index));
            }
            return path;
        }
    }
}
