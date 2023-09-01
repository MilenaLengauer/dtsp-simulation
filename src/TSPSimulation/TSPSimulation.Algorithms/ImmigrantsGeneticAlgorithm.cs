using TSPSimulation.Algorithms.Configuration;
using TSPSimulation.Algorithms.Results;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms
{
    public class ImmigrantsGeneticAlgorithm : BaseAlgorithm
    {
        private readonly int immigrants;
        private readonly int immigrantsInterval;
        private readonly bool performAfterProblemUpdate;
        private readonly ImmigrantsType immigrantsType;

        private bool problemUpdatePerformed;

        public ImmigrantsGeneticAlgorithm(TSP tsp, ImmigrantsGeneticAlgorithmConfiguration configuration, ThreadSafeRandom random) 
            : base(tsp, configuration, random)
        {
            immigrants = configuration.Immigrants;
            immigrantsInterval = configuration.ImmigrantsInterval;
            performAfterProblemUpdate = configuration.PerformAfterProblemUpdate;
            immigrantsType = configuration.ImmigrantsType;
            problemUpdatePerformed = true;
        }

        public override GenerationResult SetNewProblem(TSP tsp, int[] removedVertices, int[] addedVertices)
        { 
            if (removedVertices.Length == 0)
            {
                problemUpdatePerformed = true;
            }
            return base.SetNewProblem(tsp, removedVertices, addedVertices);
        }

        protected override GenerationResult EvolveGeneration()
        {
            TSPSolution[] childPopulation = PerformGeneticOperations(population);
            Array.Sort(childPopulation, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));


            if(CurrentGeneration % immigrantsInterval == 0 || (performAfterProblemUpdate && problemUpdatePerformed))
            {
                AddImmigrants(childPopulation);
                problemUpdatePerformed = false;
            }

            population = childPopulation;
            Array.Sort(population, (sol1, sol2) => sol1.Fitness.CompareTo(sol2.Fitness));

            return new GenerationResult(CurrentGeneration, Evaluations, population, Problem);
        }

        private void AddImmigrants(TSPSolution[] childPopulation)
        {
            switch(immigrantsType)
            {
                case ImmigrantsType.Random: AddRandomImmigrants(childPopulation); break;
                case ImmigrantsType.ElitismBased: AddElitismBasedImmigrants(childPopulation); break;
            }
        }

        private void AddElitismBasedImmigrants(TSPSolution[] childPopulation)
        {
            for(int i = 0; i < immigrants; i++)
            {
                TSPSolution immigrant = CurrentBestSolution.Clone();
                mutator.Mutate(random, immigrant);
                immigrant.Evaluate();
                Evaluations++;
                childPopulation[childPopulation.Length - 1 - i] = immigrant;
            }
        }

        private void AddRandomImmigrants(TSPSolution[] childPopulation)
        {
            for (int i = 0; i < immigrants; i++)
            {
                TSPSolution immigrant = GeneticAlgorithmUtils.CreateRandomSolution(Problem, random);
                Evaluations++;
                childPopulation[childPopulation.Length - 1 - i] = immigrant;
            }
        }

    }
}
