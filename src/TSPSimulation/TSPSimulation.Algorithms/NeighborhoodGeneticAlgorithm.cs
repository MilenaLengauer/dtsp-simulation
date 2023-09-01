using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms
{
    public class NeighborhoodGeneticAlgorithm : BaseAlgorithm
    {
        private const double TOLERANCE = 0.001;

        private readonly int m;
        private readonly double diversityEnhancementSameAsLead;
        private readonly double diversityEnhancementReplace;
        private readonly bool adaptiveNeighborhoodSize;
        private readonly int mMin;
        private readonly int mMax;
        private readonly int maxNeighborhoodCount;
        private readonly bool enableLocalSearch;
        private readonly bool mutationDiversityEnhancement;

        private int localSearchOperations = 0;

        public NeighborhoodGeneticAlgorithm(TSP tsp, NeighborhoodGeneticAlgorithmConfiguration configuration, ThreadSafeRandom random) 
            :base(tsp, configuration, random)
        {
            m = configuration.NeighborhoodSize;
            diversityEnhancementSameAsLead = configuration.DiversityEnhancementSameAsLead;
            diversityEnhancementReplace = configuration.DiversityEnhancementReplace;
            enableLocalSearch = configuration.EnableLocalSearch;
            mutationDiversityEnhancement = configuration.MutationDiversityEnhancement;
            if (configuration.NeighborhoodSizeMin.HasValue && configuration.NeighborhoodSizeMax.HasValue)
            {
                adaptiveNeighborhoodSize = true;
                mMin = configuration.NeighborhoodSizeMin.Value;
                mMax = configuration.NeighborhoodSizeMax.Value;
                maxNeighborhoodCount = (int)Math.Ceiling(populationSize / (double)mMin);
            }
            else
            {
                mMin = m;
                mMax = m;
                maxNeighborhoodCount = (int)Math.Ceiling(populationSize / (double)m);
            }
        }

        public override GenerationResult SetNewProblem(TSP tsp, int[] removedVertices, int[] addedVertices)
        {
            base.SetNewProblem(tsp, removedVertices, addedVertices);
            IList<TSPSolution[]> neighborhoods = BuildNeighborhood();
            return new NgaGenerationResult(CurrentGeneration, Evaluations, population, Problem, neighborhoods.ToArray());
        }

        protected override GenerationResult EvolveGeneration()
        {
            localSearchOperations = 0;
            IList<TSPSolution[]> neighborhoods = BuildNeighborhood();
            List<TSPSolution> childPopulation = new List<TSPSolution>();
            List <Task<TSPSolution[]>> tasks = new List<Task<TSPSolution[]>>();
            for (int i = 0; i < neighborhoods.Count; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(() => {
                    TSPSolution[] resultNeighborhood = PerformGeneticOperationsForNeighborhood(neighborhoods[idx]);
                    Array.Sort(resultNeighborhood, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));
                    return resultNeighborhood;
                }));
            }
            Task<TSPSolution[][]> resultTask = Task.WhenAll(tasks);
            resultTask.Wait();
            TSPSolution[][] childNeighborhoods = resultTask.Result;
            foreach(var neighborhood in childNeighborhoods)
            {
                childPopulation.AddRange(neighborhood);
            }
            population = childPopulation.ToArray();
            Array.Sort(population, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));

            Evaluations += (localSearchOperations * 4) / Problem.ProblemSize;

            return new NgaGenerationResult(CurrentGeneration, Evaluations, population, Problem, childNeighborhoods);
        }

        private TSPSolution[] PerformGeneticOperationsForNeighborhood(TSPSolution[] solutions)
        {
            TSPSolution[] childSolutions = new TSPSolution[solutions.Length];
            List<Task<TSPSolution>> tasks = new List<Task<TSPSolution>>();
            for (int i = 0; i < solutions.Length; i++)
            {
                tasks.Add(Task.Run(() => GeneticAlgorithmUtils.GenerateChild(solutions, selector, crossover, mutator, mutationRate, random)));
            }
            Evaluations += solutions.Length;
            Task<TSPSolution[]> resultTask = Task.WhenAll(tasks);
            resultTask.Wait();
            childSolutions = resultTask.Result;

            if (enableLocalSearch)
            {
                childSolutions = PerformLocalSearch(childSolutions);
            }

            //Replacement
            return replacement.Replace(solutions, childSolutions);
        }

        private IList<TSPSolution[]> BuildNeighborhood()
        {
            IList<TSPSolution[]> neighborhoods = new List<TSPSolution[]>();
            // duplicate population
            IList<TSPSolution> copy = new List<TSPSolution>(population);

            double currentBest = population[0].Fitness;
            double betaBest = population[maxNeighborhoodCount - 1].Fitness;

            while(copy.Count > 0)
            {
                var leader = copy[0];
                // calculate distance of every solution to leader
                IList<(int, TSPSolution)> distances = new List<(int, TSPSolution)>();
                foreach(var solution in copy)
                {
                    distances.Add((leader.Distance(solution), solution));
                }
                distances = distances.OrderBy(x => x.Item1).ToList();

                //determine neighborhood size
                int neighborhoodSize;
                if(adaptiveNeighborhoodSize)
                {
                    if(distances.Count >= mMax)
                    {
                        double changeFactor = (betaBest - currentBest) < TOLERANCE ? 0 :
                            Math.Min((leader.Fitness - currentBest) / (betaBest - currentBest), 1);
                        neighborhoodSize = (int)(changeFactor * (mMax - mMin) + mMin);
                        if (neighborhoodSize % 2 != 0) neighborhoodSize += 1;
                    } else
                    {
                        neighborhoodSize = distances.Count;
                    }
                } else
                {
                    neighborhoodSize = distances.Count >= m ? m : distances.Count;
                }

                TSPSolution[] neighborhood = new TSPSolution[neighborhoodSize];
                int countSameAsLead = 0;
                for(int i = 0; i < neighborhood.Length; i++)
                {
                    neighborhood[i] = distances[i].Item2;
                    copy.Remove(neighborhood[i]);
                    if (distances[i].Item1 == 0)
                    {
                        countSameAsLead++;
                    }
                }
                //check if diversity enhancement is necessary
                if (countSameAsLead >= neighborhood.Length * diversityEnhancementSameAsLead)
                {
                    int replaceUntil = (int) (neighborhood.Length * diversityEnhancementReplace);
                    for (int i = 1; i < replaceUntil; i++)
                    {
                        if(mutationDiversityEnhancement)
                        {
                            mutator.Mutate(random, neighborhood[i]);
                            neighborhood[i].Evaluate();
                            Evaluations++;
                        } else
                        {
                            neighborhood[i] = GeneticAlgorithmUtils.CreateRandomSolution(Problem, random);
                            Evaluations++;
                        }
                    }
                }
                Array.Sort(neighborhood, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));
                neighborhoods.Add(neighborhood);
            }
            return neighborhoods;
        }

        private TSPSolution[] PerformLocalSearch(TSPSolution[] childNeighborhood)
        {
            Array.Sort(childNeighborhood, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));
            if (childNeighborhood.Length == mMin)
            {
                // smallest possible size -> most likely already converged -> local search only on best
                childNeighborhood[0] = PerformLocalSearch(childNeighborhood[0]);
                localSearchOperations++;
            } else
            {
                // perform on better half
                for(int i = 0; i < (childNeighborhood.Length - 1) / 2; i++) 
                {
                    childNeighborhood[i] = PerformLocalSearch(childNeighborhood[i]);
                    localSearchOperations++;
                }
            }
            return childNeighborhood;
        }

        private TSPSolution PerformLocalSearch(TSPSolution solution)
        {
            TSPSolution best = solution;
            for(int i = 0; i < solution.Path.Length - 1; i++)
            {
                //swap cities next to each other -> leads to changing two edges
                TSPSolution newSolution = solution.Clone();
                int x = newSolution.Path[i];
                newSolution.Path[i] = newSolution.Path[i + 1];
                newSolution.Path[i + 1] = x;
                newSolution.Evaluate(); // could also be calculated by removing the two changed edges and adding the new ones

                if(newSolution.Fitness < best.Fitness)
                {
                    best = newSolution;
                }
            }
            return best;
        }

    }
}
