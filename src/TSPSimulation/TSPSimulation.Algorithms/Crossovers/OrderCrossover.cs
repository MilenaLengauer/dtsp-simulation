using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Crossovers
{
    public class OrderCrossover : ICrossover
    {
        public TSPSolution Crossover(ThreadSafeRandom random, TSPSolution s1, TSPSolution s2)
        {
            int[] child = new int[s1.Path.Length];

            int segmentStart = random.Next(child.Length - 1);
            int segmentEnd = random.Next(segmentStart + 1, child.Length);

            for (int i = segmentStart; i <= segmentEnd; i++)
            {
                child[i] = s1.Path[i];
            }

            int childIdx = 0;
            for (int i = 0; i < s2.Path.Length; i++)
            {
                if (childIdx == segmentStart)
                {  // skip already copied part
                    childIdx = segmentEnd + 1;
                }
                if (!child.Contains(s2.Path[i]))
                {
                    child[childIdx] = s2.Path[i];
                    childIdx++;
                }
            }

            return new TSPSolution(child, s1.Problem);
        }
    }
}
