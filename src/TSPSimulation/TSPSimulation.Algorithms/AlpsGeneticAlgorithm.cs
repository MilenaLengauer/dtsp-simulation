using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms
{
    public class AlpsGeneticAlgorithm : BaseAlgorithm
    {
        private readonly int elites;
        private readonly int numberOfLayers;
        private readonly int ageGap;
        private readonly bool plusSelection;

        private List<List<AlpsSolution>> ageLayers;

        private IList<int> layerStartAges;
        private int currentMaxAgeLayer = 0;
        private int currentAgingGeneration = 0;

        public AlpsGeneticAlgorithm(TSP tsp, AlpsGeneticAlgorithmConfiguration configuration, ThreadSafeRandom random)
            : base(tsp, configuration, random)
        {
            ageLayers = new List<List<AlpsSolution>>();
            elites = configuration.Elites;
            numberOfLayers = configuration.NumberOfLayers;
            ageGap = configuration.AgeGap;
            plusSelection = configuration.PlusSelection;
            layerStartAges = configuration.LayerStartAges();
        }

        public override GenerationResult InitializePopulation()
        {
            base.InitializePopulation();
            ageLayers = new List<List<AlpsSolution>>();
            ageLayers.Add(new List<AlpsSolution>(population.Select(s => new AlpsSolution(0, s))));
            return new AlpsGenerationResult(CurrentGeneration, Evaluations, population, Problem, ageLayers);
        }

        public override GenerationResult SetNewProblem(TSP tsp, int[] removedVertices, int[] addedVertices)
        {
            Problem = tsp;
            RepairSolutions(removedVertices, addedVertices);
            BuildPopulationFromAgeLayers();
            isOptimum = false;
            CurrentBestSolution = population[0];
            return new AlpsGenerationResult(++CurrentGeneration, ++Evaluations, population, Problem, ageLayers);
        }

        protected override AlpsGenerationResult EvolveGeneration()
        {
            currentAgingGeneration++;

            var childPopulation = new List<List<AlpsSolution>>();
            for (int i = 0; i < ageLayers.Count; i++)
            {
                List<AlpsSolution> parents = ageLayers[i];
                var matingPool = new List<AlpsSolution>(parents);
                if(i > 0)
                {
                    matingPool.AddRange(ageLayers[i - 1]);
                    matingPool = matingPool.OrderBy(s => s.Solution.Fitness).ToList();
                }
                childPopulation.Add(
                    PerformGeneticOperationsForLayer(matingPool)
                        .OrderBy(s => s.Solution.Fitness)
                        .ToList()
                );
            }

            PerformAging(childPopulation);
            ageLayers = childPopulation;
            SortLayers();

            if (currentMaxAgeLayer < numberOfLayers - 1 && currentAgingGeneration >= layerStartAges[currentMaxAgeLayer + 1])
            {
                OpenNewAgeLayer();
            }
            if (currentAgingGeneration % ageGap == 0)
            {
                ReinitializeFirstLayer();
            }

            BuildPopulationFromAgeLayers();
            return new AlpsGenerationResult(CurrentGeneration, Evaluations, population, Problem, ageLayers);
        }

        private List<AlpsSolution> PerformGeneticOperationsForLayer(List<AlpsSolution> parents)
        {
            List<AlpsSolution> childSolutions = new List<AlpsSolution>();
            TSPSolution[] solutionsArray = parents.Select(p => p.Solution).ToArray();

            int numberOfNewChildren = plusSelection ? populationSize : populationSize - elites;
            ISet<int> indicesOfSelectedParents = new HashSet<int>();

            List<Task<(AlpsSolution, int, int)>> tasks = new List<Task<(AlpsSolution, int, int)>>();

            for (int i = 0; i < numberOfNewChildren; i++)
            {
                tasks.Add(Task.Run(() => GenerateChild(parents, solutionsArray)));
            }
            Task<(AlpsSolution, int, int)[]> resultTask = Task.WhenAll(tasks);
            resultTask.Wait();
            foreach (var result in resultTask.Result)
            {
                childSolutions.Add(result.Item1);
                indicesOfSelectedParents.Add(result.Item2);
                indicesOfSelectedParents.Add(result.Item3);
            }

            //Replacement
            if (plusSelection)
            {
                var sortedChildren = childSolutions.OrderBy(s => s.Solution.Fitness).ToList();
                var selected = new List<AlpsSolution>();
                for (int i = 0, p = 0, c = 0; i < populationSize; i++)
                {
                    if (sortedChildren[c].Solution.Fitness < parents[p].Solution.Fitness)
                    {
                        selected.Add(sortedChildren[c]);
                        c++;
                    }
                    else
                    {
                        var parentClone = parents[p].Clone();
                        if (indicesOfSelectedParents.Contains(p))
                        {
                            parentClone.IncrementAge();
                        }
                        selected.Add(parentClone);
                        p++;
                    }
                }
                childSolutions = selected;
            }
            else
            {
                for (int i = 0; i < elites; i++)
                {
                    AlpsSolution eliteSolution = parents[i].Clone();
                    if (indicesOfSelectedParents.Contains(i))
                    {
                        eliteSolution.IncrementAge();
                    }
                    childSolutions.Add(eliteSolution);
                }
            }
            return childSolutions;
        }

        private (AlpsSolution, int, int) GenerateChild(List<AlpsSolution> parents, TSPSolution[] solutionsArray)
        {
            // Selection
            int idx1 = selector.Select(random, solutionsArray);
            int idx2 = selector.Select(random, solutionsArray);
            AlpsSolution s1 = parents[idx1];
            AlpsSolution s2 = parents[idx2];

            // Crossover
            TSPSolution child = crossover.Crossover(random, s1.Solution, s2.Solution);

            // Mutation
            if (random.NextDouble() < mutationRate)
            {
                mutator.Mutate(random, child);
            }
            child.Evaluate();
            Evaluations++;

            int higherParentAge = s1.Age > s2.Age ? s1.Age : s2.Age;
            return (new AlpsSolution(higherParentAge + 1, child), idx1, idx2);
        }

        private void PerformAging(List<List<AlpsSolution>> childPopulation)
        {
            for (int i = childPopulation.Count - 2; i >= 0; i--)
            {
                IList<AlpsSolution> currentLayer = childPopulation[i];
                IList<AlpsSolution> nextLayer = childPopulation[i + 1];
                IList<AlpsSolution> solutionsToRemove = new List<AlpsSolution>();
                foreach (var solution in currentLayer)
                {
                    if (solution.Age >= layerStartAges[i + 1])
                    {
                        nextLayer.Add(solution);
                        solutionsToRemove.Add(solution);
                    }
                }
                foreach (var solution in solutionsToRemove)
                {
                    currentLayer.Remove(solution);
                }
            }
        }

        private void OpenNewAgeLayer()
        {
            currentMaxAgeLayer++;
            // fill the new age layer with individuals generated from crossover + mutation from the previous layer
            List<AlpsSolution> newLayer = PerformGeneticOperationsForLayer(ageLayers.Last()).OrderBy(s => s.Solution.Fitness).ToList();
            ageLayers.Add(newLayer);
        }

        private void ReinitializeFirstLayer()
        {
            IList<AlpsSolution> newLayer = new List<AlpsSolution>();
            for (int i = 0; i < populationSize; i++)
            {
                newLayer.Add(new AlpsSolution(0, GeneticAlgorithmUtils.CreateRandomSolution(Problem, random)));
                Evaluations++;
            }
            ageLayers[0] = newLayer.OrderBy(s => s.Solution.Fitness).ToList();
        }

        private void BuildPopulationFromAgeLayers()
        {
            population = ageLayers.SelectMany(l => l).Select(s => s.Solution).OrderBy(s => s.Fitness).ToArray();
        }

        private void SortLayers()
        {
            for(int i = 0; i < ageLayers.Count; i++)
            {
                ageLayers[i] = ageLayers[i].OrderBy(s => s.Solution.Fitness).ToList();
            }
        }

        private void RepairSolutions(int[] removedVertices, int[] addedVertices)
        {
            for(int i = 0; i < ageLayers.Count; i++)
            {
                IList<AlpsSolution> layer = ageLayers[i];
                for(int j = 0; j < layer.Count; j++)
                {
                    TSPSolution repairedSolution = GeneticAlgorithmUtils.RepairSolution(Problem, layer[j].Solution, removedVertices, addedVertices);
                    layer[j] = new AlpsSolution(layer[j].Age, repairedSolution);
                }
                ageLayers[i] = layer.OrderBy(s => s.Solution.Fitness).ToList();
            }
        }
    }

    public class AlpsSolution {
        public int Age { get; private set; }
        public TSPSolution Solution { get; }

        public AlpsSolution(int age, TSPSolution solution)
        {
            Age = age;
            Solution = solution;
        } 
        
        public void IncrementAge()
        {
            Age++;
        }

        public AlpsSolution Clone()
        {
            return new AlpsSolution(Age, Solution.Clone());
        }
    }


}
