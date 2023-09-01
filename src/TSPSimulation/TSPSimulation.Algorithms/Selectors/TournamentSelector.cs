using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Selectors
{
    public class TournamentSelector : ISelector
    {
        private readonly int tournamentSize;

        public TournamentSelector(int tournamentSize)
        {
            this.tournamentSize = tournamentSize;
        }

        public int Select(ThreadSafeRandom random, TSPSolution[] population)
        {
            int parentIdx = random.Next(population.Length);
            TSPSolution parent = population[parentIdx];
            for (int j = 1; j < tournamentSize; j++)
            {
                int candidateIdx = random.Next(population.Length);
                TSPSolution candidate = population[candidateIdx];
                if (candidate.Fitness < parent.Fitness)
                {
                    parentIdx = candidateIdx;
                    parent = candidate;
                }
            }
            return parentIdx;
        }

    }
}
