using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Algorithms.Crossovers;
using TSPSimulation.Algorithms.Mutators;
using TSPSimulation.Algorithms.Replacement;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.Algorithms.Selectors;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms
{
    public abstract class BaseAlgorithm : IAlgorithm
    {
        public TSPSolution CurrentBestSolution { get; protected set; }
        public int CurrentGeneration { get; protected set; } = 0;
        public int Evaluations { get; protected set; } = 0;

        public TSP Problem { get; protected set; }

        protected readonly int populationSize;
        protected readonly double mutationRate;
        protected readonly IMutator mutator;
        protected readonly ICrossover crossover;
        protected readonly ISelector selector;
        protected readonly IReplacement replacement;
        protected readonly ThreadSafeRandom random;

        protected TSPSolution[] population;
        protected bool isOptimum;

        public BaseAlgorithm(TSP problem, AlgorithmConfiguration configuration, ThreadSafeRandom random)
        {
            Problem = problem;

            this.populationSize = configuration.PopulationSize;
            this.mutationRate = configuration.MutationRate;
            this.mutator = configuration.GetMutator();
            this.crossover = configuration.GetCrossover();
            this.selector = configuration.GetSelector();
            this.replacement = configuration.GetReplacement();
            this.random = random;

            this.population = new TSPSolution[populationSize];
            this.isOptimum = false;

            CurrentBestSolution = new TSPSolution(Problem.Cities, Problem);
        }

        public virtual GenerationResult InitializePopulation()
        {
            population = GeneticAlgorithmUtils.InitializePopulation(populationSize, Problem, random);
            Evaluations += populationSize;
            CurrentBestSolution = population[0];
            return new GenerationResult(CurrentGeneration, Evaluations, population, Problem);
        }

        public GenerationResult? Run()
        {
            GenerationResult? result = null;
            if (Problem.ProblemSize < 4)
            {
                if (!isOptimum)
                {
                    CurrentGeneration++;
                    (TSPSolution optimum, int evaluations) = GeneticAlgorithmUtils.GetOptimumThroughPermutation(Problem);
                    CurrentBestSolution = optimum;
                    Evaluations += evaluations;
                    result = new GenerationResult(CurrentGeneration, Evaluations, population, Problem);
                    isOptimum = true;
                }
            }
            else
            {
                CurrentGeneration++;
                result = EvolveGeneration();

                if (population[0].Fitness < CurrentBestSolution.Fitness)
                {
                    CurrentBestSolution = population[0];
                }
            }
            return result;
        }

        public virtual GenerationResult SetNewProblem(TSP tsp, int[] removedVertices, int[] addedVertices)
        {
            Problem = tsp;
            population = GeneticAlgorithmUtils.RepairSolutions(Problem, population, removedVertices, addedVertices);
            CurrentBestSolution = population[0];
            isOptimum = false;
            return new GenerationResult(++CurrentGeneration, ++Evaluations, population, Problem);
        }

        protected virtual GenerationResult EvolveGeneration()
        {
            TSPSolution[] childPopulation = PerformGeneticOperations(population);
            population = childPopulation;
            Array.Sort(population, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));
            return new GenerationResult(CurrentGeneration, Evaluations, population, Problem);
        }

        protected TSPSolution[] PerformGeneticOperations(TSPSolution[] solutions)
        {
            TSPSolution[] childSolutions = new TSPSolution[populationSize];
            List<Task<TSPSolution>> tasks = new List<Task<TSPSolution>>();
            for (int i = 0; i < populationSize; i++)
            {
                tasks.Add(Task.Run(() => GeneticAlgorithmUtils.GenerateChild(solutions, selector, crossover, mutator, mutationRate, random)));
            }
            Evaluations += populationSize;
            Task<TSPSolution[]> resultTask = Task.WhenAll(tasks);
            resultTask.Wait();
            childSolutions = resultTask.Result;
            //Replacement
            return replacement.Replace(solutions, childSolutions);
        }

    }
}
