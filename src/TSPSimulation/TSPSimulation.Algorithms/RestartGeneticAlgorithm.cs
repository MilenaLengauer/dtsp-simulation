using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms
{
    public class RestartGeneticAlgorithm : BaseAlgorithm
    {

        public RestartGeneticAlgorithm(TSP tsp, RestartGeneticAlgorithmConfiguration configuration, ThreadSafeRandom random) 
            : base(tsp, configuration, random)
        {
        }

        public override GenerationResult SetNewProblem(TSP tsp, int[] removedVertices, int[] addedVertices)
        {
            Problem = tsp;
            RepairSolutions(removedVertices, addedVertices);
            CurrentBestSolution = population[0];
            isOptimum = false;
            return new GenerationResult(++CurrentGeneration, ++Evaluations, population, Problem);
        }

        private void RepairSolutions(int[] removedVertices, int[] addedVertices)
        {
            if (removedVertices.Length > 0)
            {
                for (int i = 0; i < population.Length; i++)
                {
                    TSPSolution solution = population[i];
                    // remove vertices
                    List<int> newPath = solution.Path.Where(v => !removedVertices.Contains(v)).ToList();
                    TSPSolution newSolution = new TSPSolution(newPath.ToArray(), Problem);
                    newSolution.Evaluate();
                    population[i] = newSolution;
                }
                Array.Sort(population, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));
            }
            else
            {
                // re-initialize population for the new problem
                InitializePopulation();
            }
        }
    }
}
