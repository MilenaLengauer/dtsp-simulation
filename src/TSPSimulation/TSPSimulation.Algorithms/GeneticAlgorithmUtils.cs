using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Crossovers;
using TSPSimulation.Algorithms.Mutators;
using TSPSimulation.Algorithms.Selectors;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms
{
    public static class GeneticAlgorithmUtils
    {
        public static TSPSolution CreateRandomSolution(TSP problem, ThreadSafeRandom random)
        {
            int[] randomPath = (int[])problem.Cities.Clone();
            randomPath.Shuffle(random);

            TSPSolution solution = new TSPSolution(randomPath, problem);
            solution.Evaluate();
            return solution;
        }

        public static TSPSolution[] InitializePopulation(int size, TSP problem, ThreadSafeRandom random)
        {
            TSPSolution[] population = new TSPSolution[size];
            for (int i = 0; i < size; i++)
            {
                population[i] = CreateRandomSolution(problem, random);
            }
            Array.Sort(population, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));
            return population;
        }

        public static TSPSolution GenerateChild(TSPSolution[] parents, ISelector selector, ICrossover crossover, IMutator mutator, double mutationRate, ThreadSafeRandom random)
        {
            // Selection
            TSPSolution p1 = parents[selector.Select(random, parents)];
            TSPSolution p2 = parents[selector.Select(random, parents)];
            // Crossover
            TSPSolution child = crossover.Crossover(random, p1, p2);
            // Mutation
            if (random.NextDouble() < mutationRate)
            {
                mutator.Mutate(random, child);
            }
            child.Evaluate();
            return child;
        }

        /// <summary>
        /// Repairs the solutions in the current population after a vertice-change.
        /// removed vertices are deleted from the solutions
        /// added vertices are inserted into the existing solution - minimize d(x,k) + d(k,y) - d(x,y)
        /// </summary>
        /// <param name="removedVertices"></param>
        /// <param name="addedVertices"></param>
        public static TSPSolution[] RepairSolutions(TSP problem, TSPSolution[] population, int[] removedVertices, int[] addedVertices)
        {
            TSPSolution[] repairedPopulation = new TSPSolution[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                repairedPopulation[i] = RepairSolution(problem, population[i], removedVertices, addedVertices);
            }
            Array.Sort(repairedPopulation, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));
            return repairedPopulation;
        }

        public static TSPSolution RepairSolution(TSP problem, TSPSolution solution, int[] removedVertices, int[] addedVertices)
        {
            // remove vertices
            List<int> newPath = solution.Path.Where(v => !removedVertices.Contains(v)).ToList();

            // add vertices
            foreach (int addedVertice in addedVertices)
            {
                double bestInsertionValue = double.MaxValue;
                int insertionIdx = 0;
                int city1, city2;
                if (newPath.Count > 0)
                {
                    for (int j = 0; j <= newPath.Count; j++)
                    {
                        if (j == 0)
                        {
                            city1 = solution.Problem.Start;
                            city2 = newPath[j];
                        }
                        else if (j == newPath.Count)
                        {
                            city1 = newPath[j - 1];
                            city2 = solution.Problem.End;
                        }
                        else
                        {
                            city1 = newPath[j - 1];
                            city2 = newPath[j];
                        }

                        double insertionValue = problem.DistanceMatrix[city1, addedVertice] + problem.DistanceMatrix[city2, addedVertice]
                            - problem.DistanceMatrix[city1, city2];
                        if (insertionValue < bestInsertionValue)
                        {
                            bestInsertionValue = insertionValue;
                            insertionIdx = j;
                        }
                    }
                }
                newPath.Insert(insertionIdx, addedVertice);
            }
            TSPSolution newSolution = new TSPSolution(newPath.ToArray(), problem);
            newSolution.Evaluate();
            return newSolution;
        }

        public static (TSPSolution, int) GetOptimumThroughPermutation(TSP problem)
        {
            int evaluations = 0;
            var permutations = problem.Cities.GetPermutations();
            var solution = new TSPSolution(problem.Cities, problem);
            solution.Evaluate();
            double bestFitness = solution.Fitness;
            foreach (var permutation in permutations)
            {
                var testSolution = new TSPSolution(permutation.ToArray(), problem);
                testSolution.Evaluate();
                evaluations++;
                if (testSolution.Fitness < bestFitness)
                {
                    solution = testSolution;
                    bestFitness = solution.Fitness;
                }
            }
            return (solution, evaluations);
        }

    }
}
