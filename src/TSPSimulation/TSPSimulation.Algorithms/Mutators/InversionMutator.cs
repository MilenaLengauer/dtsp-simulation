using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Mutators
{
    public class InversionMutator : IMutator
    {
        public void Mutate(ThreadSafeRandom random, TSPSolution tspSolution)
        {
            if (tspSolution.Path.Length < 2) return;
            int idx1 = random.Next(tspSolution.Path.Length);
            int idx2;
            do
            {
                idx2 = random.Next(tspSolution.Path.Length);
            } while (idx1 == idx2);
            if (idx2 < idx1)
            {
                int h = idx1;
                idx1 = idx2;
                idx2 = h;
            }
            // reverse
            for (int i = idx1, j = idx2; i < j; i++, j--)
            {
                int h = tspSolution.Path[i];
                tspSolution.Path[i] = tspSolution.Path[j];
                tspSolution.Path[j] = h;
            }
        }
    }
}
