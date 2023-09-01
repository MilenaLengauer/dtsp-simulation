using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSPSimulation.Commons;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.Algorithms.Mutators
{
    public class ExchangeMutator : IMutator
    {
        public void Mutate(ThreadSafeRandom random, TSPSolution tspSolution)
        {
            if (tspSolution.Path.Length < 2) return;
            int idx1 = random.Next(tspSolution.Path.Length);
            int idx2;
            do
            {
                idx2 = random.Next(tspSolution.Path.Length);
            } while (idx1 == idx2 /*Math.Abs(idx1 - idx2) <= 1*/);

            int h = tspSolution.Path[idx1];
            tspSolution.Path[idx1] = tspSolution.Path[idx2];
            tspSolution.Path[idx2] = h;
        }
    }
}
